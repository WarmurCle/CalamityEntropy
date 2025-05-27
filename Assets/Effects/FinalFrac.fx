sampler uImage : register(s0);
float4 color1;
float4 color2;
float4 EffectFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 colory = tex2D(uImage, coords);
    return lerp(color1, color2, colory.r) * float4(1, 1, 1, colory.r);
}

technique Technique1
{
    pass EffectPass
    {
        PixelShader = compile ps_2_0 EffectFunction();
    }
}