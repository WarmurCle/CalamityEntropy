sampler uImage0 : register(s0);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;
float2 uTargetPosition;
float4 uLegacyArmorSourceRect;
float2 uLegacyArmorSheetSize;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    static const float2 PIXEL_SIZE = 2.0f / uImageSize0;
    static const float THRESHOLD = 0.35f;

    float4 color = tex2D(uImage0, coords);
    
    // 快速边界检测
    if (coords.x < PIXEL_SIZE.x || coords.x > 1.0f - PIXEL_SIZE.x ||
        coords.y < PIXEL_SIZE.y || coords.y > 1.0f - PIXEL_SIZE.y) {
        return float4(uColor.rgb, 1) * color.a * sampleColor.a;
    }
    
    float a1 = tex2D(uImage0, coords + float2(0.0f, PIXEL_SIZE.y)).a;
    float a2 = tex2D(uImage0, coords + float2(-PIXEL_SIZE.x, 0.0f)).a;
    float a3 = tex2D(uImage0, coords + float2(PIXEL_SIZE.x, 0.0f)).a;
    float a4 = tex2D(uImage0, coords + float2(0.0f, -PIXEL_SIZE.y)).a;
    
    if (a1 < THRESHOLD || a2 < THRESHOLD || a3 < THRESHOLD || a4 < THRESHOLD) {
        return float4(uColor.rgb, 1) * color.a * sampleColor.a;
    }
    
    return float4(uSecondaryColor.rgb, 1) * color.a * sampleColor.a;
}

technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}