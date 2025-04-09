sampler uImage : register(s0);
sampler colorMap : register(s1);
float4 color1;
float4 color2;
float alpha;
float ofs;
float4 EffectFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float2 cd = float2(frac(coords.x + ofs), coords.y);
    if(tex2D(uImage, cd).r < 1 - coords.x)
    {
        return float4(0, 0, 0, 0);
    }
    return lerp(color1, color2, tex2D(uImage, cd).r) * tex2D(colorMap, coords).a * alpha;
}

technique Technique1
{
    pass EffectPass
    {
        PixelShader = compile ps_2_0 EffectFunction();
    }
}