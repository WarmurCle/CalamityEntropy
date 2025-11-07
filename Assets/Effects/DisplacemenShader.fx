// 传入的由粒子拼成的通道材质（主贴图）
sampler InPutTexture : register(s0);
// 噪声置换图
sampler uDisplacementSampler : register(s1);

// 修正：颜色变量改为 float4（RGBA 四通道）
float uTime;                // 时间变量（控制动画）
float uIntensity;           // 扭曲强度
float4 uBaseColor;          // 基础颜色
float4 uTargetColor;        // 目标颜色
float uColorFactor;         // 颜色渐变权重(0→1)
float2 uNoiseScale;         // 噪声图UV缩放因子（二维向量）
float uFadeRange = 0.3f;    // 底部淡出范围(0~1)
bool UseColor;              // 是否染色

float4 DisplacementFunction(float2 coords : TEXCOORD0) : COLOR0
{
    // 1. 噪声UV计算：先缩放，再叠加时间动画（修正逻辑）
    float2 noiseUV = coords * uNoiseScale; // 噪声缩放（保留细节）
    noiseUV += float2(0, -uTime * 0.1);    // 仅叠加Y轴时间滚动（不重复加原坐标）

    // 2. 噪声采样与扭曲计算
    float displacement = tex2D(uDisplacementSampler, noiseUV).r;
    // 对称扭曲（避免单向偏移）
    float2 offset = float2(0, (displacement - 0.5) * 2 * uIntensity);
    // 限制UV范围，防止采样越界黑边
    float2 displacedUV = saturate(coords + offset);

    // 3. 主贴图采样
    float4 originalColor = tex2D(InPutTexture, displacedUV);
    //4.底部淡出度计算
    //Coords.y > (1 - uFadeRage)，淡出
    float fadeFactor = 1.0f;
    if(coords.y > (1.0f - uFadeRange))
    {
        fadeFactor = lerp(1.0f,0.0f,(coords.y - (1.0f - uFadeRange)) / uFadeRange);
    }
    // 4. 颜色混合（支持开关，避免空值）
    float4 finalColor = originalColor;
    if (UseColor)
    {
        float4 mixedColor = lerp(uBaseColor, uTargetColor, uColorFactor);
        finalColor = originalColor * mixedColor;
        // 透明度修正（匹配颜色渐变）
        finalColor.a = originalColor.a * lerp(uBaseColor.a, uTargetColor.a, uColorFactor);
    }

    return finalColor;
}

// 最终渲染通道（兼容 ps_2_0，确保工具支持）
technique Technique1
{
    pass LPADisplacementPass
    {
        PixelShader = compile ps_2_0 DisplacementFunction();
    }
}