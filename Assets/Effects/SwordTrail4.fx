sampler uImage : register(s0);
sampler uImage1 : register(s1);
float uTime;
float4 color1;
float4 color2;
float alpha;
float4 EffectFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 colory = tex2D(uImage, coords);
    return (lerp(color1, color2, tex2D(uImage1, float2(frac(coords.x + uTime * 0.06), coords.y)).r)) * float4(1, 1, 1, colory.r * alpha);
}

technique Technique1
{
    pass EffectPass
    {
        PixelShader = compile ps_2_0 EffectFunction();
    }
}