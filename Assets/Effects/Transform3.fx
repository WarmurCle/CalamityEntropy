sampler uImage : register(s0);  sampler uTransformImage : register(s1);  float uTime;  float4 color;
float4 baseColor = float4(1, 1, 1, 1);
float strength = 0.6;
 float4 EnchantedFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 colory = tex2D(uImage, coords) * baseColor;

	     if (!any(colory))
    {
        return colory;
    }
	float alpha = colory.a;
    colory.a = 1;
	     float2 barCoord = float2((coords.x * 0.1 - uTime * 0.5) % 1.0, (coords.y * 0.1 + uTime) % 1.0);
    if (barCoord.x < 0)
    {
        barCoord.x = 1 + barCoord.x;
    }
	
	     return clamp((tex2D(uTransformImage, barCoord) * strength * color + colory) * alpha, 0, 1) * baseColor;
}

technique Technique1
{
    pass EnchantedPass
    {
        PixelShader = compile ps_2_0 EnchantedFunction();
    }
}