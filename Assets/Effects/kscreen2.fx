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
float i;
float2 offset;
float4 PSFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float4 color2 = tex2D(uImage1, coords);
    float4 color3 = tex2D(uImage2, float2(coords.x + offset.x, coords.y + offset.y));
    if (!any(color2))
        return color;
    else
    {
        float2 vec = float2(0, 0);
        float rot = color3.r * 6.283;
        vec = float2(cos(rot), sin(rot)) * (color3.r - 0.5) * i * 0.7;
        return tex2D(uImage0, coords + vec);

    }
}
technique Technique1
{
    pass kscreen2
    {
        PixelShader = compile ps_2_0 PSFunction();
    }
}
