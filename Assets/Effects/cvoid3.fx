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
float2 scsize;
float time;
float4 PSFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float2 pcoords = coords;
    float4 color2 = tex2D(uImage1, pcoords);
    float4 color3 = tex2D(uImage2, float2(frac(coords.x + offset.x), frac(coords.y + offset.y)));
    if (!any(color2))
    {
        return float4(0, 0, 0, 0);
    }
    else
    {
        float4 rt = color3;
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
