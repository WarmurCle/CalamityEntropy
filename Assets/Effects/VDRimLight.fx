//缘带贴在内侧 2 个 texel,外扩不由这个着色器做,贴图四边留 2 到 4 像素空白,输出预乘 alpha 并用 Additive 混合,噪声放在 s1
sampler uImage0 : register(s0);
sampler uImage1 : register(s1);

float uTime;
float uOpacity;//该值可以大于 1
float3 uColor;
float3 uHotColor;
float uHeat;
float uFlash;
float uErode;//该值越高丝状越碎,爆闪时把它压回实心
float2 uNoiseScroll;//该值平移直角层噪声的 UV,宿主把 wrap 限制在 0 到 1
float uRadialScroll;//该值是极坐标径向位移,大于 0 向外,小于 0 向内
float uRadialMix;//该值是极坐标层的权重,范围 0 到 1
float2 uFrameCenter;//该值是当前帧的 UV 中心,条带图不是 0.5
float2 uImageSize;

float4 PixelFunc(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 c = tex2D(uImage0, coords);
    float2 px = 2.0 / uImageSize;//取样锁在 2 个 texel,条带帧高 122、行距 124,再大就会串到别的帧

    float aL = tex2D(uImage0, coords - float2(px.x, 0.0)).a;
    float aR = tex2D(uImage0, coords + float2(px.x, 0.0)).a;
    float aU = tex2D(uImage0, coords - float2(0.0, px.y)).a;
    float aD = tex2D(uImage0, coords + float2(0.0, px.y)).a;
    float amin = min(min(aL, aR), min(aU, aD));

    //斜向取样用 0.7071,y 方向 1.41 个 texel 仍落在帧间隙里
    float2 d = px * 0.7071;
    float aLU = tex2D(uImage0, coords - d).a;
    float aRD = tex2D(uImage0, coords + d).a;
    float aRU = tex2D(uImage0, coords + float2(d.x, -d.y)).a;
    float aLD = tex2D(uImage0, coords + float2(-d.x, d.y)).a;
    amin = min(amin, min(min(aLU, aRD), min(aRU, aLD)));

    float edge = saturate(c.a - amin);

    float2 pix = coords * uImageSize;

    float nA = tex2D(uImage1, frac(pix / 72.0 - uNoiseScroll)).r;
    float nA2 = tex2D(uImage1, frac(pix / 130.0 + float2(0.31, 0.77) + uNoiseScroll)).r;
    float cart = nA * 0.6 + nA2 * 0.4;

    //角向铺 8 个整数瓦片,接缝处的 frac 相等
    float2 rel = pix - uFrameCenter * uImageSize;
    float ang = atan2(rel.y, rel.x) / 6.2831853 + 0.5;
    float rad = length(rel) / 72.0;
    float nB = tex2D(uImage1, frac(float2(ang * 8.0, rad - uRadialScroll))).r;
    float nB2 = tex2D(uImage1, frac(float2(ang * 5.0 + 0.37, rad * 0.55 + uRadialScroll))).r;
    float polar = nB * 0.6 + nB2 * 0.4;

    float wisp = smoothstep(0.26, 0.76, lerp(cart, polar, uRadialMix));
    edge *= lerp(1.0, wisp, uErode);

    float heat = saturate(uHeat + uFlash);
    float3 col = lerp(uColor, uHotColor, heat);
    float lum = dot(c.rgb, float3(0.3, 0.59, 0.11));
    col += uColor * lum * 0.25;

    float flicker = 1.0 + uFlash * 0.25 * sin(uTime * 40.0);
    float a = edge * uOpacity * (1.0 + uFlash) * flicker;
    return float4(col * a, a) * baseColor;
}

technique Technique1
{
    pass RimPass
    {
        PixelShader = compile ps_3_0 PixelFunc();
    }
}
