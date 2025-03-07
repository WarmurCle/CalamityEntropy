sampler uImage : register(s0);
float prog;
float alpha = 1;
float4 EnchantedFunction(float2 coords : TEXCOORD0) : COLOR0
{
    if(any(tex2D(uImage, coords))){
        return lerp(tex2D(uImage, coords), float4(1, 1, 1, 1), prog) * tex2D(uImage, coords).a * alpha;
    }
    return tex2D(uImage, coords);
}

technique Technique1
{
    pass EnchantedPass
    {
        PixelShader = compile ps_2_0 EnchantedFunction();
    }
}