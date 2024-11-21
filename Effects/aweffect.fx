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

float a;
float alpha;

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 originalColor = tex2D(SpriteTextureSampler, input.TextureCoordinates);   
    if (any(originalColor)){
        return lerp(originalColor, float4(1, 1, 1, originalColor.a), a) * alpha;
    }
    else{
        return float4(0, 0, 0, 0);
    }
}

technique Technique1
{
    pass aweffect
    {
        PixelShader = compile ps_2_0 MainPS();
    }
};
