Texture2D SpriteTexture;
sampler2D SpriteTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};


float4 MainPS(VertexShaderOutput input) : COLOR
{
    return lerp(float4(1, 0, 0, 1), float4(0, 0, 1, 1), input.TextureCoordinates.x);
}

technique Technique1
{
    pass test
    {
        PixelShader = compile ps_2_0 MainPS();
    }
};
