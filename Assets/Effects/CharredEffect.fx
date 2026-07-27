sampler uImage : register(s0); 
sampler noise : register(s1); 
float minAlpha;
float4 cColor;
float2 noiseSize;
float2 texSize;
float2 noiseOffset;

float4 ShaderPass(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage, coords);

    if (!any(color) || minAlpha == 1)
    {
        return color;
    }
    float2 nCoords = frac((coords * texSize + noiseOffset) / noiseSize);
    float4 ncolor = tex2D(noise, nCoords);
    float clerp = 0;
    if(ncolor.r > minAlpha)
    {
        clerp = (ncolor.r - minAlpha) / (1 - minAlpha);
    }
    return lerp(color, float4(cColor.r, cColor.g, cColor.b, color.a), clerp * cColor.a) * baseColor;
}

technique Technique1
{
    pass EnchantedPass
    {
        PixelShader = compile ps_2_0 ShaderPass();
    }
}