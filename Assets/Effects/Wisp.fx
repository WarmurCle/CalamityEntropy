sampler uImage : register(s0); 
float min;
float max;
float4 minColor;
float4 maxColor;

float4 EnchantedFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 colory = tex2D(uImage, coords);

    if (!any(colory))
    {
        return colory;
    }
    return lerp(minColor, maxColor, (colory.r + colory.g + colory.b - min) / (max - min));
}

technique Technique1
{
    pass EnchantedPass
    {
        PixelShader = compile ps_2_0 EnchantedFunction();
    }
}