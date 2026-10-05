//coords.x 沿射线 0..1,coords.y 横向 0..1,输出 Additive,噪声在 s1
sampler uImage0 : register(s0);
sampler uImage1 : register(s1);

float uTime;
float3 uColor;
float3 uColor2;
float uEnvelope;//宽度包络 0..1
float uLength;//长宽比,噪声按它铺
float uOpacity;
float uSeed;

float4 PixelFunc(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 baseTex = tex2D(uImage0, coords);
    float along = coords.x;
    float across = coords.y * 2.0 - 1.0;
    float tile = max(uLength, 1.0);

    float n1 = tex2D(uImage1, float2(frac(along * tile * 0.5 - uTime * 1.7 + uSeed), frac(coords.y * 0.9 + uSeed))).r;
    float n2 = tex2D(uImage1, float2(frac(along * tile * 0.23 + uTime * 0.9 + uSeed * 2.0), frac(coords.y * 0.6 + uTime * 0.3))).r;
    float noise = n1 * 0.6 + n2 * 0.4;

    float halfWidth = max(uEnvelope, 0.001) * (0.72 + 0.28 * noise);
    float d = abs(across) / halfWidth;
    float body = saturate(1.0 - d);
    float core = pow(saturate(1.0 - d * 1.8), 4.0);
    float rim = pow(saturate(1.0 - abs(d - 0.85) * 6.0), 2.0) * 0.6;

    float cap = smoothstep(0.0, 0.04, along) * (1.0 - smoothstep(0.92, 1.0, along));

    float3 col = uColor * (body * (0.55 + 0.45 * noise)) + uColor2 * core + uColor * rim + core * 0.6;
    float a = saturate(body * 0.85 + core + rim) * cap * uOpacity * baseTex.a;
    return float4(col * a, a) * baseColor;
}

technique Technique1
{
    pass BeamPass
    {
        PixelShader = compile ps_3_0 PixelFunc();
    }
}
