Texture2D SpriteTexture;
sampler2D TextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
};

float2 resolution;  float blurAmount;   
struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
         float4 color = float4(0, 0, 0, 0);
    float total = 0.0;
    
    float weights[8] = { 0.153170, 0.144893, 0.122649, 0.092902, 0.123452, 0.134543, 0.145314, 0.143513};
    float2 offsets[8] = { float2(-1, -1), float2(0, -1), float2(1, -1), float2(-1, 0), float2(1, 0), float2(-1, 1), float2(0, 1), float2(1, 1) };
    for (int i = 0; i < 8; i++)
    {
        float2 offset = offsets[i] * blurAmount / resolution;
        color += tex2D(TextureSampler, input.TextureCoordinates + offset) * weights[i];
        color += tex2D(TextureSampler, input.TextureCoordinates - offset) * weights[i];
        total += weights[i] * 2.0;
    }
    
    return color / total;
}

technique GaussianBlur
{
    pass P0
    {
        PixelShader = compile ps_2_0 MainPS();
    }
}