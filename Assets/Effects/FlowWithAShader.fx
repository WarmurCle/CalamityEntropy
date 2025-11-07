// 传入的背景材质
sampler FlowTexture : register(s0);
sampler AlphaTexture : register(s1);

float2 FlowTextureSize;
// 长度
float2 targetSize;
// 偏移
float uTime;
// 用于染色的颜色 (从C#代码传入)
float4 uColor;
bool Color;

float4 FlowWithAFunction(float2 coords : TEXCOORD0) : COLOR0
{ 
    // 根据材质与坐标指定的基础颜色
    float4 baseColor = tex2D(AlphaTexture, coords);
    
    // 如果没有颜色，则不做处理
    if (!any(baseColor))
        discard;
    
    // 根据材质与坐标指定的基础颜色
    // 转化为目标的像素坐标
    float2 pixelPos = coords * targetSize;
    // 将对应像素坐标转换为激光材质的UV坐标
    float2 bgUV = pixelPos / FlowTextureSize;
    // 流动
    bgUV = float2(bgUV.x + uTime * 0.01, bgUV.y);
    // 模一个1限制到0-1之间
    bgUV = frac(bgUV);
   // 采样流动的激光颜色
    float4 finalColor = tex2D(FlowTexture, bgUV);
    // 将采样出的颜色与我们传入的 uColor 相乘
    // finalColor.rgb *= uColor.rgb; // 仅染色RGB通道
    finalColor *= uColor; // 染色的同时，也受 uColor 的 Alpha 值影响
    
    return finalColor;
}

// 最终通道
technique Technique1
{
    pass UCAFlowWithAPass
    {
        PixelShader = compile ps_2_0 FlowWithAFunction();
    }
}