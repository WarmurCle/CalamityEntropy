//s0 流动底图,s1 遮罩,UV 按 targetSize/FlowTextureSize 再加 uTime
sampler FlowTexture : register(s0);
sampler AlphaTexture : register(s1);

float2 FlowTextureSize;
float2 targetSize;//长度
float uTime;//偏移
float4 uColor;//染色
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