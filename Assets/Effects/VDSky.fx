//距离落在按屏高归一的平面上,x 再乘宽高比,输出预乘 alpha 并用 AlphaBlend 混合,uFlip 为 1 时上下翻转,着色器里没有动态分支
sampler uImage0 : register(s0);   //uImage0 是全屏白方块,这里只取它的 UV
sampler uImage1 : register(s1);   //uImage1 提供星云和侵蚀,采样方式是 LinearWrap
sampler uImage2 : register(s2);   //uImage2 是星球表面,采样方式是 LinearWrap

float uTime;
float uOpacity;         //该值用存在包络乘上强度
float uAspect;          //该值是画面宽除以高
float uFlip;            //该值为 1 时表示反重力
float2 uParallax;       //该值是相机位置除以屏高
float uPhase;           //该值取 1 到 3,配色由 C# 按它插好
float uErosion;         //该值是星球被侵蚀的程度,范围 0 到 1
float uFlash;           //该值是拍点闪光强度,范围 0 到 1
float uFlashRing;       //该值是冲击环半径,单位是屏高,小于 0 时没有环
float uCharge;          //该值是主炮蓄力,范围 0 到 1
float2 uBossUv;         //该值是本体在屏幕上的 UV
float uBossGlow;        //该值是核心亮度,范围 0 到 1
float uGridRadius;      //该值是网格亮化半径,单位是屏高
float uGridAlpha;       //该值是网格的基础亮度
float uGridCell;        //该值是六边形格的边长,单位是屏高
float2 uPlanetCenter;   //该值是星球圆心的 UV
float uPlanetRadius;    //该值是星球半径,单位是屏高
float uPlanetParallax;  //该值是星球的视差系数
float uPlanetSpin;      //该值是自转速度,单位是圈每秒
float uRingSpin;        //该值是碎屑环的绕行速度,单位是圈每秒
float2 uStarParallax;   //该值是星野视差,x 给远层,y 给近层
float uNebulaParallax;  //该值是星云的视差系数
float3 uColorTop;
float3 uColorHorizon;
float3 uColorNebula;
float3 uColorGrid;
float3 uColorErosion;
float3 uColorPlanetRim;
float3 uColorRing;

//hash21 先把输入压小,格 id 用 fmod 对 512 取模,乘数一大星星会排成格子
float hash21(float2 p)
{
    p = frac(p * float2(127.1, 311.7));
    p += dot(p, p + 45.32);
    return frac(p.x * p.y);
}

//cells 是每屏高有多少格,density 是有星的比例,每格放一颗星
float StarLayer(float2 p, float cells, float density, float seed)
{
    float2 g = p * cells;
    //视差相对开打原点归零,正负 20 屏以内不会撞上 fmod 的负数分支
    float2 id = fmod(floor(g) + 4096.0, 512.0);
    float2 f = frac(g) - 0.5;
    float h = hash21(id + seed);
    float2 off = (float2(hash21(id + seed + 7.1), hash21(id + seed + 3.3)) - 0.5) * 0.6;
    float d = length(f - off);
    float present = step(1.0 - density, h);
    float size = 0.04 + 0.05 * hash21(id + seed + 11.7);
    float star = present * saturate(1.0 - d / size);
    star *= star;
    float tw = 0.7 + 0.3 * sin(uTime * (1.0 + 2.0 * h) + h * 6.2831 + seed);
    return star * tw;
}

//返回的 x 是到最近格边的距离,0 在边上,0.5 在格心,yz 是格 id,传入的 p 必须为正
float3 HexCoords(float2 p)
{
    float2 r = float2(1.0, 1.7320508);
    float2 h = r * 0.5;
    float2 a = fmod(p, r) - h;
    float2 b = fmod(p - h, r) - h;
    float2 gv = lerp(b, a, step(dot(a, a), dot(b, b)));
    float2 q = abs(gv);
    float hd = max(dot(q, normalize(float2(1.0, 1.7320508))), q.x);
    float2 id = p - gv;
    return float3(0.5 - hd, id);
}

float4 PixelFunc(float2 uv : TEXCOORD0) : COLOR0
{
    float asp = max(uAspect, 0.1);
    float sy = lerp(uv.y, 1.0 - uv.y, uFlip);
    float2 sp = float2(uv.x * asp, uv.y);
    float2 bp = float2(uBossUv.x * asp, uBossUv.y);

    float3 col = lerp(uColorTop, uColorHorizon, smoothstep(0.1, 1.0, sy));
    float2 nuv1 = sp * 0.9 + uParallax * uNebulaParallax + float2(uTime * 0.004, uTime * 0.002);
    float2 nuv2 = sp * 1.7 - uParallax * uNebulaParallax * 0.6 + float2(-uTime * 0.003, uTime * 0.005) + 0.37;
    float n1 = tex2D(uImage1, frac(nuv1)).r;
    float n2 = tex2D(uImage1, frac(nuv2)).r;
    float neb = n1 * n2;
    col += uColorNebula * neb * 0.55;

    float2 starP = sp + (bp - sp) * uCharge * 0.05;
    float sFar = StarLayer(starP + uParallax * uStarParallax.x, 34.0, 0.35, 0.0);
    float sNear = StarLayer(starP + uParallax * uStarParallax.y + 13.7, 20.0, 0.25, 2.0);
    col += float3(0.75, 0.7, 1.0) * (sFar * 0.45 + sNear * 0.65);

    float bd = length(sp - bp);
    float r2 = max(uGridRadius * uGridRadius, 0.0001);
    float bossMask = exp(-bd * bd / r2) * (0.35 + 0.65 * uBossGlow);
    //冲击环的亮度随半径自己衰减,不跟着 uFlash 一起变暗
    float ringOn = step(0.0, uFlashRing);
    float ringD = (bd - uFlashRing) / 0.06;
    float ringFade = saturate(1.0 - uFlashRing / 2.0);
    float shock = exp(-ringD * ringD) * ringOn * ringFade;

    float2 pc = float2(uPlanetCenter.x * asp, lerp(uPlanetCenter.y, 1.0 - uPlanetCenter.y, uFlip))
        - uParallax * uPlanetParallax
        + float2(sin(uTime * 0.13), cos(uTime * 0.09)) * 0.006;
    float2 pp = (sp - pc) / max(uPlanetRadius, 0.01);
    float d = length(pp);
    float disk = 1.0 - smoothstep(0.985, 1.0, d);
    float z = sqrt(saturate(1.0 - d * d));
    float3 nrm = float3(pp.x, pp.y, z);
    float lon = atan2(nrm.x, nrm.z) / 6.2831853 + uTime * uPlanetSpin;
    float lat = asin(clamp(nrm.y, -1.0, 1.0)) / 3.1415927 + 0.5;
    float2 suv = float2(lon, lat);
    float terrain = tex2D(uImage2, frac(suv * float2(2.0, 1.0))).r * 0.6 + tex2D(uImage2, frac(suv * float2(5.0, 2.5) + 0.31)).r * 0.4;
    float continents = smoothstep(0.42, 0.6, terrain);
    float3 light = normalize(float3(-0.55, -0.45, 0.7));
    float ndl = saturate(dot(nrm, light));
    float limb = pow(z, 0.6);
    float rimBreath = 0.85 + 0.15 * sin(uTime * 0.8);
    float3 albedo = lerp(float3(0.045, 0.03, 0.075), float3(0.20, 0.15, 0.30), terrain) + float3(0.06, 0.03, 0.09) * continents;
    float3 planet = albedo * (0.3 + 0.7 * ndl) * limb + uColorPlanetRim * pow(1.0 - z, 3.0) * 0.4 * rimBreath;
    float2 euv = suv * float2(3.0, 1.5) + 0.13;
    float en = tex2D(uImage1, frac(euv)).r * 0.8 + tex2D(uImage1, frac(euv * 2.1 + 0.41)).r * 0.2;
    float eatField = en * 0.85 + 0.15 * (0.5 + 0.5 * pp.y);
    float thr = 1.0 - uErosion * 0.95;
    float eaten = smoothstep(thr - 0.04, thr + 0.04, eatField);
    float edge = saturate(1.0 - abs(eatField - thr) / 0.07);
    edge *= edge * step(0.02, uErosion);
    float flow = tex2D(uImage1, frac(euv * 2.5 + float2(uTime * 0.05, uTime * 0.11))).r;
    float crawl = 0.55 + 0.9 * flow;
    float pulse = 0.85 + 0.15 * sin(uTime * 2.7 + en * 12.0);
    float3 eatenCol = float3(0.01, 0.0, 0.03) + uColorErosion * (0.04 + 0.16 * flow * en);
    float3 planetCol = lerp(planet, eatenCol, eaten) + uColorErosion * edge * crawl * pulse * (0.9 + uFlash * 1.2);
    planetCol += uColorGrid * shock * 0.25;
    float2 q = float2(pp.x, pp.y * 3.2);
    float rr = length(q);
    float band = smoothstep(1.3, 1.45, rr) * (1.0 - smoothstep(1.85, 2.1, rr));
    float rang = atan2(q.y, q.x) / 6.2831853 + uTime * uRingSpin;
    float2 ruv = float2(rang, rr * 2.0);
    float rn = tex2D(uImage1, frac(ruv * float2(4.0, 1.0))).r;
    float rn2 = tex2D(uImage1, frac(ruv * float2(9.0, 2.0) + 0.5)).r;
    float ringDensity = band * (rn * 0.7 + rn2 * 0.5) * (0.5 + 0.5 * smoothstep(0.3, 0.7, rn));
    float2 rcell = floor(float2(frac(rang) * 64.0, (rr - 1.3) * 12.0));
    float glint = step(0.93, hash21(rcell)) * band * (0.6 + 0.4 * sin(uTime * 5.0 + rcell.x));
    float ringVis = lerp(1.0 - disk, 1.0, step(0.0, pp.y));//近侧在 pp.y 大于 0 时压在盘前面,远侧被 disk 挡住
    float3 ringCol = (uColorRing * ringDensity * 0.7 + uColorErosion * glint * 0.6) * ringVis;
    col = lerp(col, planetCol, disk);
    col += ringCol;
    float halo = exp(-max(d - 1.0, 0.0) * 5.0) * (1.0 - disk);
    col += uColorErosion * halo * (0.05 + 0.10 * uErosion) * rimBreath;

    float3 hc = HexCoords(sp / max(uGridCell, 0.005) + 100.0);
    float edgeDist = hc.x;
    float edgeLine = 1.0 - smoothstep(0.0, 0.05, edgeDist);
    float cellHash = hash21(hc.yz);
    float breathe = 0.6 + 0.4 * sin(uTime * 1.3 + cellHash * 6.2831);
    float gridI = uGridAlpha * breathe + bossMask * 0.35 + uFlash * 0.45 + shock * 0.9 + uCharge * 0.2;
    float interior = smoothstep(0.02, 0.45, edgeDist);
    float cellFill = uCharge * exp(-bd * bd / 0.06) * (0.25 + 0.5 * cellHash) * 0.35;
    col += uColorGrid * (edgeLine * gridI + interior * cellFill);
    col += uColorGrid * uFlash * 0.04;

    return float4(col * uOpacity, uOpacity);
}

technique Technique1
{
    pass SkyPass
    {
        PixelShader = compile ps_3_0 PixelFunc();
    }
}
