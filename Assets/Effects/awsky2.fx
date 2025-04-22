sampler uImage : register(s0);
float time;

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};
float4 EnchantedFunction(VertexShaderOutput input) : COLOR
{
    float2 coords = input.TextureCoordinates;
    float4 yc = tex2D(uImage, coords);
    return tex2D(uImage, coords + float2(sin(yc.x + time * - 0.37) * 0.1, cos(1 - yc.y + time * 0.3) * 0.1));
}

technique Technique1
{
    pass EnchantedPass
    {
        PixelShader = compile ps_2_0 EnchantedFunction();
    }
}