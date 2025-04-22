sampler uImage : register(s0);
float f1;
float f2;
texture2D colorMap;
sampler uImage1 = sampler_state
{
    Texture = <colorMap>;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};
float strength;
float offset;

float4 EnchantedFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 colory = tex2D(uImage, coords);

    if (!any(colory))
    {
        return colory;
    }
    return (colory + tex2D(uImage1, float2(frac(coords.x + offset * 0.3f), frac(((coords.y - f1) / (f2 - f1)) * 0.2 + offset)))) * baseColor;
}

technique Technique1
{
    pass EnchantedPass
    {
        PixelShader = compile ps_2_0 EnchantedFunction();
    }
}