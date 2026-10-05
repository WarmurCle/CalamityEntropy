//这个着色器按宽高比绕 Center 扭转 UV,FadeOutDistance 取 0 到 1
sampler TextureSampler : register(s0);

float2 Center;
float Strength;
float AspectRatio;
float FadeOutDistance; //渐隐从该半径开始,范围 0 到 1
float FadeOutWidth;    //该值决定渐隐带有多宽
float2 TexOffset;
float enhanceLightAlpha;

float4 PixelShaderFunction(float4 baseColor : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
    float2 adjustedTexCoord = texCoord - Center;
    adjustedTexCoord.y /= AspectRatio;
    
    float distance = length(adjustedTexCoord);
    
    float alpha = 1.0;
    if (distance > FadeOutDistance)
    {
        alpha = 1.0 - smoothstep(FadeOutDistance, FadeOutDistance + FadeOutWidth, distance);
    }
    
    float angle = atan2(adjustedTexCoord.y, adjustedTexCoord.x);
    angle += Strength * distance;
    
    adjustedTexCoord.x = cos(angle) * distance;
    adjustedTexCoord.y = sin(angle) * distance;
    
    adjustedTexCoord.y *= AspectRatio;
    adjustedTexCoord += Center;
    
    float4 color = tex2D(TextureSampler, adjustedTexCoord + TexOffset);
    if(color.r > enhanceLightAlpha)
    {
        float z = enhanceLightAlpha + (color.r - enhanceLightAlpha) * 3.5;
        color = float4(z, z, z, color.a);
    }
    color *= float4(alpha, alpha, alpha, alpha);
    
    return color * baseColor;
}

technique VortexTechnique
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}