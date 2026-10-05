//s1 的噪声扰动采样位置,输出预乘 alpha 并用 AlphaBlend 混合
sampler uImage0 : register(s0);
sampler uImage1 : register(s1);

float uTime;
float uOpacity;
float uFog;//该值是雾量,范围 0 到 1,对应 VDDepth.FogAmount
float3 uFogColor;
float uDesat;//该值是去饱和程度,范围 0 到 1
float2 uBlur;//该值是模糊半径,单位是 UV
float uShimmer;//该值是噪声扰动幅度,单位是 UV

float4 PixelFunc(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 n = tex2D(uImage1, coords * 0.8 + float2(uTime * 0.06, -uTime * 0.04)).rg - 0.5;
    float2 uv = coords + n * uShimmer * (0.4 + 0.6 * uFog);

    float4 c = tex2D(uImage0, uv) * 2.0;
    c += tex2D(uImage0, uv + float2(uBlur.x, 0.0));
    c += tex2D(uImage0, uv - float2(uBlur.x, 0.0));
    c += tex2D(uImage0, uv + float2(0.0, uBlur.y));
    c += tex2D(uImage0, uv - float2(0.0, uBlur.y));
    c /= 6.0;

    float lum = dot(c.rgb, float3(0.3, 0.59, 0.11));
    float3 rgb = lerp(c.rgb, float3(lum, lum, lum), uDesat);
    rgb = lerp(rgb, uFogColor * (0.5 + lum), uFog);

    float a = c.a * uOpacity;
    return float4(rgb * a, a) * baseColor;
}

technique Technique1
{
    pass DepthFogPass
    {
        PixelShader = compile ps_3_0 PixelFunc();
    }
}
