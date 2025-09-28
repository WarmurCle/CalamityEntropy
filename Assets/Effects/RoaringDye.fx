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
    static const float2 OFFSET_SCALE = float2(2.0f, 2.0f) / uImageSize0;

    float4 colory = tex2D(uImage0, coords);
    
    float2 offset1 = OFFSET_SCALE;
    
    float a1 = tex2D(uImage0, coords + float2(0.0f, offset1.y)).a;
    float a2 = tex2D(uImage0, coords + float2(-offset1.x, 0.0f)).a;
    float a3 = tex2D(uImage0, coords + float2(offset1.x, 0.0f)).a;
    float a4 = tex2D(uImage0, coords + float2(0.0f, -offset1.y)).a;
    float4 alphaTest = float4(a1, a2, a3, a4);
    bool anyBelowThreshold = any(alphaTest < 0.35);
    
    return ((anyBelowThreshold ? uColor : uSecondaryColor) * colory.a * sampleColor);
}

technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}