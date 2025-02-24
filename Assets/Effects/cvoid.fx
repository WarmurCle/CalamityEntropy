sampler uImage1 : register(s0);
texture2D tex1;
texture2D tex2;
texture2D tex3;
texture2D tex4;
texture2D tex5;
texture2D tex6;
sampler uImage2 = sampler_state
{
    Texture = <tex1>;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};
sampler uImage3 = sampler_state
{
    Texture = <tex2>;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};
sampler uImage4 = sampler_state
{
    Texture = <tex3>;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};
sampler uImage5 = sampler_state
{
    Texture = <tex4>;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};
sampler uImage6 = sampler_state
{
    Texture = <tex5>;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};
sampler uImage7 = sampler_state
{
    Texture = <tex6>;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};
float2 offset;
float2 scsize;
float time;
float4 PSFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float2 pcoords = floor(coords * (scsize / 2)) / (scsize / 2);
    float4 color2 = tex2D(uImage1, pcoords);
    float4 color3 = tex2D(uImage2, float2(frac(coords.x + offset.x), frac(coords.y + offset.y)));
    float4 color4 = tex2D(uImage3, float2(frac(coords.x + offset.x + time * -0.001), frac(coords.y + offset.y + time * 0.001)));
    float4 color5 = tex2D(uImage4, float2(frac(coords.x + offset.x + time * -0.002), frac(coords.y + offset.y + time * 0.002)));
    float4 color6 = tex2D(uImage5, float2(frac(coords.x + offset.x + time * -0.003), frac(coords.y + offset.y + time * 0.003)));
    float4 color7 = tex2D(uImage6, float2(frac(coords.x + offset.x + time * -0.004), frac(coords.y + offset.y + time * 0.004)));
    float4 color8 = tex2D(uImage7, float2(frac(coords.x + offset.x + time * -0.005), frac(coords.y + offset.y + time * 0.005)));
    if (!any(color2))
    {
        float xj = 2.0 / scsize.x;
        float yj = 2.0 / scsize.y;
        if (any(tex2D(uImage1, pcoords + float2(-xj, 0))) || any(tex2D(uImage1, pcoords + float2(xj, 0))) || any(tex2D(uImage1, pcoords + float2(0, yj))) || any(tex2D(uImage1, pcoords + float2(0, -yj))))
        {
            return float4(0.6, 0, 0.6, 0.725);
        }
        return float4(0, 0, 0, 0);
    }
    else
    {
        float4 rt = color3;
        rt += color4;
        rt += color5;
        rt += color6;
        rt += color7;
        rt += color8;
        rt.rgb = clamp(rt.rgb, 0.0, 1.0);
        return rt;
    }
}
technique Technique1
{
    pass cvoid
    {
        PixelShader = compile ps_2_0 PSFunction();
    }
}
