//coords.x 是沿轴的像素,coords.y 是带符号的横向像素,不等宽梯形不用 0 到 1 的 UV,输出预乘 alpha 并用 Additive 混合,噪声放在 s1
sampler uImage1 : register(s1);

float uTime;
float3 uColor;
float3 uColor2;
float uEnvelope;//该值是可见半宽相对四边形半宽的比例
float uOpacity;
float uSeed;
float uLengthPx;//该值是轴长,单位是屏幕像素
float uHalfStart;//该值是起点的四边形半宽,单位是屏幕像素
float uHalfEnd;//该值是终点的四边形半宽,单位是屏幕像素
float uWStart;//该值是起点的 1/Scale(Z),在平面上为 1,越远越大
float uWEnd;//该值是终点的 1/Scale(Z),在平面上为 1,越远越大
float uTile;//该值是噪声沿全长铺多少块
float uFogStart;//该值是起点雾量,由 C# 按 VDDepth 算好
float uFogEnd;//该值是终点雾量
float uAlphaStart;//该值是起点的深度透明度
float uAlphaEnd;//该值是终点的深度透明度
float3 uFogColor;
float2 uCap;//该值是两端端帽的渐隐比例,按屏幕 t,C# 把它钳到不小于 0.002

float4 PixelFunc(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float t = saturate(coords.x / max(uLengthPx, 1.0));
    float halfQuad = max(lerp(uHalfStart, uHalfEnd, t), 0.001);
    float across = coords.y / halfQuad;
    float v = across * 0.5 + 0.5;

    //1/w 在屏幕上是线性的,这里解出沿轴参数 f
    float f = t * uWStart / max((1.0 - t) * uWEnd + t * uWStart, 0.0001);

    float tile = max(uTile, 1.0);
    float n1 = tex2D(uImage1, float2(frac(f * tile * 0.5 - uTime * 1.7 + uSeed), frac(v * 0.9 + uSeed))).r;
    float n2 = tex2D(uImage1, float2(frac(f * tile * 0.23 + uTime * 0.9 + uSeed * 2.0), frac(v * 0.6 + uTime * 0.3))).r;
    float noise = n1 * 0.6 + n2 * 0.4;

    float halfWidth = max(uEnvelope, 0.001) * (0.72 + 0.28 * noise);
    float d = abs(across) / halfWidth;
    float body = saturate(1.0 - d);
    float core = pow(saturate(1.0 - d * 1.8), 4.0);
    float rim = pow(saturate(1.0 - abs(d - 0.85) * 6.0), 2.0) * 0.6;

    float cap = smoothstep(0.0, uCap.x, t) * (1.0 - smoothstep(1.0 - uCap.y, 1.0, t));

    float3 col = uColor * (body * (0.55 + 0.45 * noise)) + uColor2 * core + uColor * rim + core * 0.6;

    float fog = lerp(uFogStart, uFogEnd, f);
    float lum = dot(col, float3(0.333, 0.333, 0.333));
    col = lerp(col, uFogColor * (lum + 0.15), fog);
    float aZ = lerp(uAlphaStart, uAlphaEnd, f);

    float a = saturate(body * 0.85 + core + rim) * cap * uOpacity * aZ;
    return float4(col * a, a) * baseColor;
}

technique Technique1
{
    pass TaperedPass
    {
        PixelShader = compile ps_3_0 PixelFunc();
    }
}
