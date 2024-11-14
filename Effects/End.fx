sampler2D RenderTargetSampler : register(s0); // RenderTarget 纹理
sampler2D ImageSampler : register(s1); // 目标图片纹理

struct PS_INPUT
{
    float4 Position : SV_POSITION;
    float2 TexCoord : TEXCOORD0;
};

float4 MainPS(PS_INPUT input) : SV_Target
{
    // 从RenderTarget读取当前像素的颜色
    float4 renderTargetColor = tex2D(RenderTargetSampler, input.TexCoord);
    float4 imageColor = tex2D(ImageSampler, input.TexCoord);
    // 如果当前像素是完全不透明的
    if (renderTargetColor.a > 0.0f)
    {
        // 从目标图片读取对应坐标的颜色
        
        // 将RenderTarget的颜色替换为目标图片的颜色
        return imageColor * renderTargetColor.a;
    }
    else
    {
        // 保持原RenderTarget的颜色
        return renderTargetColor;
    }
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 MainPS();
    }
}
