//按 uColor 染色,Additive,透明处返回 0
sampler uImage0 : register(s0);

float uTime;
float uOpacity;
float3 uColor;
float2 uImageSize;//贴图像素尺寸

float4 PixelFunc(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 c = tex2D(uImage0, coords);
    float mask = step(0.01, c.a);

    float lum = dot(c.rgb, float3(0.3, 0.59, 0.11));
    float3 col = uColor * (0.35 + lum * 0.9);

    float scan = 0.72 + 0.28 * sin(coords.y * uImageSize.y * 1.5708 + uTime * 6.0);
    float flicker = 0.92 + 0.08 * sin(uTime * 21.0);

    float2 px = 1.0 / uImageSize;
    float aL = tex2D(uImage0, coords - float2(px.x, 0)).a;
    float aR = tex2D(uImage0, coords + float2(px.x, 0)).a;
    float aU = tex2D(uImage0, coords - float2(0, px.y)).a;
    float aD = tex2D(uImage0, coords + float2(0, px.y)).a;
    float edge = saturate(4.0 * c.a - aL - aR - aU - aD);
    col += uColor * edge * 0.9 + edge * 0.35;

    float a = c.a * uOpacity * scan * flicker * mask;
    return float4(col * a, a) * baseColor;
}

technique Technique1
{
    pass HologramPass
    {
        PixelShader = compile ps_3_0 PixelFunc();
    }
}
