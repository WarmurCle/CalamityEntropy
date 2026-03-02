sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler noise : register(s2);
float iTime;
float strengthMult;
float2 screen;
float4 PSFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float strength = strengthMult * tex2D(uImage1, coords).r;
    float4 n1 = tex2D(noise, frac(coords + float2(iTime + 0.4, iTime + 0.7) + screen));
    float4 n2 = tex2D(noise, frac(coords + float2(-iTime + 0.3, -iTime + 0.5) + screen));
    float2 offset = float2(n1.r - 0.5, n2.r - 0.5) * strength * 0.2;
    
    float4 color = tex2D(uImage0, coords + offset);
    return color;
}
technique Technique1
{
    pass fscreen
    {
        PixelShader = compile ps_2_0 PSFunction();
    }
}
