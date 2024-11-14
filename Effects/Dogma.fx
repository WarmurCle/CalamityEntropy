Texture2D SpriteTexture;
float r1;
float r2;
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

// 简单的伪随机数生成函数
float rand(float2 co)
{
    return frac(sin(dot(co.xy ,float2(r1, r2))) * 43758.5453);
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 originalColor = tex2D(SpriteTextureSampler, input.TextureCoordinates);   
    // 生成随机颜色
    float4 Main.randColor;
    float f = rand((input.TextureCoordinates * 100) % 2);
    Main.randColor.r = f;
    Main.randColor.g = f;
    Main.randColor.b = f;
    Main.randColor.a = originalColor.a;
    
    return Main.randColor * originalColor.a;
}

technique RandomColorDrawing
{
    pass P0
    {
        PixelShader = compile ps_2_0 MainPS();
    }
};
