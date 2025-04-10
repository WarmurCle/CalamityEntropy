sampler uImage : register(s0);  float strength;
float alpha = 1;
float4 EnchantedFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 colory = tex2D(uImage, coords);

    return clamp(colory * lerp(float4(1, 1, 1, 1), float4(0.8, 0.6, 0.6, 1), strength), 0, 1) * alpha;
}

technique Technique1
{
    pass EffectPass
    {
        PixelShader = compile ps_2_0 EnchantedFunction();
    }
}