sampler uImage : register(s0);
float4 clr1;
float4 clr2;
float4 ShaderFunc(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 colory = tex2D(uImage, coords);

    if (!any(colory))
    {
        return colory;
    }
	float4 clr = colory * baseColor;
    if(coords.x < 0.5)
        clr = lerp(clr1, clr, coords.x * 2);
    if(coords.x > 0.5)
        clr = lerp(clr2, clr, (1 - coords.x) * 2);
    return  clr * baseColor.a;
}

technique Technique1
{
    pass EnchantedPass
    {
        PixelShader = compile ps_2_0 ShaderFunc();
    }
}