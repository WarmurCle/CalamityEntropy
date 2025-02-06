sampler uImage0 : register(s0);
texture2D tex0;
texture2D tex1;
sampler uImage1 = sampler_state
{
    Texture = <tex0>;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};
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
float4 PSFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float4 color2 = tex2D(uImage1, coords);
    float4 color3 = tex2D(uImage2, float2(frac(coords.x + offset.x), frac(coords.y + offset.y)));
    float4 color4 = tex2D(uImage2, float2(frac(-(coords.x + offset.x + time * -0.009)), frac(coords.y + offset.y + time * 0.007)));
    float4 color5 = tex2D(uImage2, float2(frac(coords.x + offset.x + time * 0.007), frac(-(coords.y + offset.y + time * -0.009))));
    if (!any(color2))
        return color;
    else
    {
        return lerp(color, color3 * clr + color4 * clr + color5 * clr, color2.a);

    }
}
technique Technique1
{
    pass cabyss
    {
        PixelShader = compile ps_2_0 PSFunction();
    }
}
