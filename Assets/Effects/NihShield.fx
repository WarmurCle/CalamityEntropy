sampler uImage : register(s0); 
sampler uImage1 : register(s1); 
float offset;
float num;
float4 OutlineColor;

float4 EnchantedFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 colory = tex2D(uImage, coords);
    float4 colorS = tex2D(uImage1, coords + float2(offset, offset)) + tex2D(uImage1, coords + float2(offset / 2, offset / 2));
    float dist = distance(coords, float2(0.5, 0.5)) * 2 * num;
    float alpha = dist;
    alpha *= alpha;
    return (colorS * alpha * baseColor + OutlineColor * dist * dist) * colory.r;
}

technique Technique1
{
    pass TechPass
    {
        PixelShader = compile ps_2_0 EnchantedFunction();
    }
}