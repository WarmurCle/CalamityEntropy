sampler uImage0 : register(s0);
texture2D tex0;
sampler uImage1 = sampler_state
{
    Texture = <tex0>;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};
float minAlpha = 1.2;
float a;
float r;
float g;
float b;
float4 PSFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float4 color2 = tex2D(uImage1, coords);
    if((color.r + color.g + color.b) > minAlpha){
        return color2 * lerp(float4(0, 0, 0, 0), float4(r, g, b, 1), color.a) * a;      }
    else{
        if((color.r + color.g + color.b) > minAlpha - 0.38){
            return lerp(color2 * lerp(float4(0, 0, 0, 0), float4(r, g, b, 1), color.a), color2 * lerp(float4(0, 0, 0, 0), float4(1, 0.9, 0.12, 1), color.a), (((minAlpha - (color.r + color.g + color.b))) / 0.38)) * a * ((0.38 - (minAlpha - (color.r + color.g + color.b))) / 0.38);
        }
        return float4(0, 0, 0, 0);
    }
}
technique Technique1
{
    pass AWSkyEffect
    {
        PixelShader = compile ps_2_0 PSFunction();
    }
}
