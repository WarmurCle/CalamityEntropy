sampler uImage : register(s0);

float4 EnchantedFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 colory = tex2D(uImage, frac(coords));
    return float4(colory.b, colory.g, colory.r, 1) * baseColor * colory.a;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 EnchantedFunction();
    }
}