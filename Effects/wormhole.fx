sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;
float4 uShaderSpecificData;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (!any(color))
        return color;
    // pos 就是中心了
    float2 pos = float2(0.5, 0.5);
    // offset 是中心到当前点的向量
    float2 offset = (coords - pos);
    // 因为长宽比不同进行修正
    float2 rpos = offset * float2(uScreenResolution.x / uScreenResolution.y, 1);
    float dis = length(rpos);
    // 向量长度缩短0.8倍
    return tex2D(uImage0, pos + offset * 0.8);
}
technique Technique1
{
    pass P0
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}