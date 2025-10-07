sampler uImage1 : register(s0);
texture2D tex1;
sampler uImage2 = sampler_state
{
    Texture = <tex1>;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};
float2 offset;
float time;
float4 clr;
float2 scrsize;

float4 PSFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float2 pcoords = floor(coords * (scrsize / 2)) / (scrsize / 2);
    float4 color2 = tex2D(uImage1, pcoords);
    float4 color3 = tex2D(uImage2, float2(frac(coords.x + offset.x), frac(coords.y + offset.y)));
    float4 color4 = tex2D(uImage2, float2(frac(-(coords.x + offset.x + time * -0.009)), frac(coords.y + offset.y + time * 0.007)));
    float4 color5 = tex2D(uImage2, float2(frac(coords.x + offset.x + time * 0.007), frac(-(coords.y + offset.y + time * -0.009))));
    if (!any(color2))
    {
        float2 j = float2(2, 2) / scrsize;
        if(any(tex2D(uImage1, pcoords + float2(-j.x, 0))) || any(tex2D(uImage1, pcoords + float2(j.x, 0))) || any(tex2D(uImage1, pcoords + float2(0, j.y))) || any(tex2D(uImage1, pcoords + float2(0, -j.y))))
        {
            return float4(6, 0, 0, 1);
        }
        return float4(0, 0, 0, 0);
    }
    else
    {
        return color3 * clr + color4 * clr + color5 * clr;
    }
}
technique Technique1
{
    pass cabyss
    {
        PixelShader = compile ps_2_0 PSFunction();
    }
}
