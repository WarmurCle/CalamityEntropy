sampler uImage0 : register(s0);
float2 scsize;

float4 PSFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float2 pcoords = floor(coords * (scsize / 2)) / (scsize / 2);
    return tex2D(uImage0, pcoords);// * sampleColor;
}
technique Technique1
{
    pass Pixel
    {
        PixelShader = compile ps_2_0 PSFunction();
    }
}
