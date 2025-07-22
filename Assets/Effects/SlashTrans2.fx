sampler uImage : register(s0);
sampler uTransformImage : register(s1);
float ofs;
float4 EnchantedFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 colory = tex2D(uImage, frac(coords + float2(ofs, 0)));
    float4 barColor = tex2D(uTransformImage, float2(0.5, coords.y));
	
    return (lerp(float4(0.2, 0.2, 0.5, 0.46), float4(0.7, 0.7, 1, 1), colory.b) * barColor) * (coords.x * coords.y) * colory * baseColor;
}

technique Technique1
{
    pass EnchantedPass
    {
        PixelShader = compile ps_2_0 EnchantedFunction();
    }
}