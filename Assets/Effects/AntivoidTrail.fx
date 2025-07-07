sampler uImage : register(s0);
sampler uTrail : register(s1);
sampler pLight : register(s2);
float alpha;
float offset;
float4 EffectFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color1 = tex2D(uImage, float2(offset + coords.x, coords.y));
    float4 color2 = tex2D(uTrail, coords);
    float4 color3 = tex2D(pLight, coords);
    float z = 0;
    if(color2.r > 0.7)
    {
        z = (color2.r - 0.7) * 3;
    }
    return ((color2 * float4(0.2, 0.06, 0.3, color2.r + z)) + (color1 * float4(1, 0.24, 0.586, color2.r + z)) * color2.r + color3 * float4(0.3, 0.2, 0.8, 0.2 * color3.r * (color2.r + z))) * float4(1, 1, 1, alpha);
}

technique Technique1
{
    pass EffectPass
    {
        PixelShader = compile ps_2_0 EffectFunction();
    }
}