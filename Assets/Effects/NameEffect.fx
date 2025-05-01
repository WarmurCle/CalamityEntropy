sampler uImage : register(s0);  
sampler uTransformImage : register(s1); 
float uTime;

 float4 EnchantedFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 colory = tex2D(uImage, coords);

	if (!any(colory))
    {
        return colory;
    }
	return clamp(lerp(tex2D(uImage, coords), float4(0.4, 0.4, 1, 1), (0.5 + 0.5 * sin(uTime)) * 0.3) + tex2D(uTransformImage, float2(frac(coords.x * 0.3 + uTime * 0.46), coords.y)) * 0.7, 0, 1);
}

technique Technique1
{
    pass EnchantedPass
    {
        PixelShader = compile ps_2_0 EnchantedFunction();
    }
}