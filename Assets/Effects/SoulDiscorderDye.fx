sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;
float2 uTargetPosition;
float4 uLegacyArmorSourceRect;
float2 uLegacyArmorSheetSize;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 colory = tex2D(uImage0, coords);

    if (!any(colory))
    {
        return colory;
    }
    return (colory + tex2D(uImage1, float2(frac(coords.x + uTime * 0.3f), frac(((coords.y - uSourceRect[1] / uImageSize0[1]) / (uSourceRect[3] / uImageSize0[1])) * 0.2 + uTime)))) * sampleColor * float4(colory.a, colory.a, colory.a, colory.a);
}

technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}