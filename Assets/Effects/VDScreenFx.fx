//全屏 UV 已经按 GameViewMatrix 折好,uOpacity 为 0 时这张滤镜被跳过,着色器里没有动态分支
sampler uImage0 : register(s0);

float uOpacity;//原版把这个值喂进来,关掉像素效果时它是 0
float uTime;
float2 uLensCenter;//该值是透镜中心的 UV
float uLensStrength;//该值决定径向拉扯有多强
float uLensRadius;//该值是高斯模糊的半径
float4 uRift[3];//该数组存三段裂隙的端点 UV,xy 是起点,zw 是终点
float4 uRiftOpen;//该值是三段裂隙各自的开口量
float uVignette;//该值把画面四周压暗
float uImpact;//该值标记当前是不是冲击帧
float4 uWhoosh;//xy 是中心 UV,z 是强度,中心一圈不做拖影
float uAspect;//该值是画面宽高比

float4 PixelFunc(float2 uv : TEXCOORD0) : COLOR0
{
    float4 orig = tex2D(uImage0, uv);
    float2 asp = float2(max(uAspect, 0.1), 1.0);

    float2 toC = (uv - uLensCenter) * asp;
    float dist = length(toC);
    float radius = max(uLensRadius, 0.001);
    float lensFall = exp(-dist * dist / (radius * radius));
    float pull = uLensStrength * lensFall;
    float2 dirC = toC / max(dist, 0.0005);
    float2 offset = -dirC * pull * radius / asp;
    float darken = saturate(uLensStrength * 5.0 * exp(-dist * dist / (radius * radius * 0.16)));

    float2 riftOff = float2(0.0, 0.0);
    float riftGlow = 0.0;
    float opens[3] = { uRiftOpen.x, uRiftOpen.y, uRiftOpen.z };
    [unroll]
    for (int i = 0; i < 3; i++)
    {
        float2 a = uRift[i].xy;
        float2 b = uRift[i].zw;
        float2 ab = (b - a) * asp;
        float2 ap = (uv - a) * asp;
        float t = saturate(dot(ap, ab) / max(dot(ab, ab), 0.00001));
        float2 perp = ap - ab * t;
        float dperp = length(perp);
        float open = opens[i];
        float width = 0.0009 * open + 0.00002;
        float band = exp(-dperp * dperp / width) * step(0.001, open);
        float2 pushDir = perp / max(dperp, 0.0005);
        riftOff += pushDir / asp * band * open * 0.03;
        riftGlow += band * open;
    }

    float2 disp = offset + riftOff;
    float2 finalUv = uv + disp;
    float chroma = saturate(riftGlow + pull * 6.0);
    float4 col;
    col.r = tex2D(uImage0, finalUv + disp * 0.35 * chroma).r;
    col.g = tex2D(uImage0, finalUv).g;
    col.b = tex2D(uImage0, finalUv - disp * 0.35 * chroma).b;
    col.a = 1.0;

    float2 toW = (uv - uWhoosh.xy) * asp;
    float wd = length(toW);
    float2 wdir = toW / max(wd, 0.0005);
    float wAmt = uWhoosh.z * saturate(wd * 1.6) * 0.045;
    float4 streak = col;
    [unroll]
    for (int k = 1; k <= 6; k++)
    {
        float2 suv = finalUv - wdir / asp * wAmt * k / 6.0;
        streak += tex2D(uImage0, suv);
    }
    streak /= 7.0;
    col.rgb = lerp(col.rgb, streak.rgb, saturate(uWhoosh.z * 1.2));

    col.rgb *= 1.0 - darken * 0.75;
    col.rgb += float3(0.85, 0.72, 1.0) * saturate(riftGlow) * 0.9;

    float vig = length((uv - 0.5) * asp);
    float vigMask = smoothstep(0.35, 0.95, vig);
    col.rgb *= 1.0 - uVignette * vigMask * 0.85;
    col.rgb = lerp(col.rgb, col.rgb * float3(0.75, 0.6, 0.95), uVignette * 0.5);

    float lum = dot(col.rgb, float3(0.3, 0.59, 0.11));
    float bw = smoothstep(0.32, 0.62, lum);
    col.rgb = lerp(col.rgb, float3(bw, bw, bw), saturate(uImpact));

    return lerp(orig, col, saturate(uOpacity));
}

technique Technique1
{
    pass ScreenFxPass
    {
        PixelShader = compile ps_3_0 PixelFunc();
    }
}
