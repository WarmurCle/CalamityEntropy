sampler uImage : register(s0); 
float strength;
float4 minColor;
float4 maxColor;

float4 EnchantedFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 colory = tex2D(uImage, coords);

    if (!any(colory))
    {
        return colory;
    }
    return lerp(colory, lerp(minColor, maxColor, (colory.r + colory.g + colory.b) / 3), strength) * baseColor;
}

technique Technique1
{
    pass EnchantedPass
    {
        PixelShader = compile ps_2_0 EnchantedFunction();
    }
}