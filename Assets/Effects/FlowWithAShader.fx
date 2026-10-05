//s0 提供流动底图,s1 提供遮罩,UV 先按 targetSize 除以 FlowTextureSize 缩放,再加上 uTime
sampler FlowTexture : register(s0);
sampler AlphaTexture : register(s1);

float2 FlowTextureSize;
float2 targetSize;//targetSize 是底图要铺开的像素尺寸
float uTime;//uTime 推动 UV 偏移
float4 uColor;//uColor 给流动结果染色
bool Color;

float4 FlowWithAFunction(float2 coords : TEXCOORD0) : COLOR0
{ 
    float4 baseColor = tex2D(AlphaTexture, coords);
    
    if (!any(baseColor))
        discard;
    
    float2 pixelPos = coords * targetSize;
    float2 bgUV = pixelPos / FlowTextureSize;
    bgUV = float2(bgUV.x + uTime * 0.01, bgUV.y);
    bgUV = frac(bgUV);
    float4 finalColor = tex2D(FlowTexture, bgUV);
    // finalColor.rgb *= uColor.rgb; // 仅染色RGB通道
    finalColor *= uColor;
    
    return finalColor;
}

technique Technique1
{
    pass UCAFlowWithAPass
    {
        PixelShader = compile ps_2_0 FlowWithAFunction();
    }
}