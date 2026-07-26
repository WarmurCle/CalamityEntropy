sampler uImage : register(s0);

float rad;
float2 center;

float4 PixelShaderFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 uv = coords;
    
    float2 centered = uv - center;
    
    float cosA = cos(rad);
    float sinA = sin(rad);
    float2 originalCentered = float2(
        centered.x * cosA + centered.y * sinA,
       -centered.x * sinA + centered.y * cosA
    );
    
    float2 originalUV = originalCentered + center;
    
    if (originalUV.x < 0.0 || originalUV.x > 1.0 ||
        originalUV.y < 0.0 || originalUV.y > 1.0)
    {
        return float4(0.0, 0.0, 0.0, 0.0);
    }
    
    float4 texColor = tex2D(uImage, originalUV);
    return texColor * baseColor;
}

technique Technique1
{
    pass EnchantedPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}