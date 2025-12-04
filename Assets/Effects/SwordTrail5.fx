sampler uImage : register(s0);
sampler uAlphaSample : register(s1);
float4 color1;
float4 color2;
float alpha;
float xoffset;
float4 EffectFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 colory = tex2D(uImage, float2(coords.x + xoffset, coords.y));
    float4 color3 = float4(0, 0, 0, 0);
    return (lerp(color1, color2, colory.r) + color3) * float4(1, 1, 1, colory.r * alpha) * coords.x * tex2D(uAlphaSample, coords).r;
}

technique Technique1
{
    pass EffectPass
    {
        PixelShader = compile ps_2_0 EffectFunction();
    }
}