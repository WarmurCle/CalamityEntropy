sampler uImage : register(s0);
sampler uTransformImage : register(s1);
float4 EnchantedFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 colory = tex2D(uImage, coords);

    if (!any(colory))
    {
        return colory;
    }
    float2 barCoord = float2(colory.r, 0.5);
	
    return tex2D(uTransformImage, barCoord) * colory;
}

technique Technique1
{
    pass EnchantedPass
    {
        PixelShader = compile ps_2_0 EnchantedFunction();
    }
}