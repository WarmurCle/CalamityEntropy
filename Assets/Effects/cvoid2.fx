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
float4 PSFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color0 = tex2D(uImage0, coords);
    float4 color = tex2D(uImage1, coords);
    float4 color2 = tex2D(uImage2, float2(frac(coords.x - offset.x * 1), frac(coords.y - offset.y * 1)));
    float4 color3 = tex2D(uImage2, float2(frac(coords.x + offset.x * 1), frac(coords.y + offset.y * 1)));
    if (!any(color) || (color.r == 0 && color.g == 0 && color.b == 0))
        return color0;
    else
    {
        float4 rt = (color2 + color3) * 0.5;
        rt.a = 1;
        rt = float4(color.r * rt.r, color.g * rt.g, color.b * rt.b, rt.a);
        return lerp(color0, rt, color.a);

    }
}
technique Technique1
{
    pass cvoid
    {
        PixelShader = compile ps_2_0 PSFunction();
    }
}
