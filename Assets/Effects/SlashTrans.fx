sampler uImage : register(s0);
sampler uTransformImage : register(s1);
float4 EnchantedFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 colory = tex2D(uImage, coords);
    float4 barColor = tex2D(uTransformImage, coords);
	
    return colory * (barColor + float4(0.24 * barColor.a, 0.24 * barColor.a, 0.24 * barColor.a, 0)) * baseColor;
}

technique Technique1
{
    pass EnchantedPass
    {
        PixelShader = compile ps_2_0 EnchantedFunction();
    }
}