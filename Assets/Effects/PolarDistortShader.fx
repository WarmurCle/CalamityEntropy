//这个着色器把 UV 折成极坐标,角向乘 uWidthMult,径向乘 uRingMult,s1 提供 alpha
sampler source : register(s0);//source 采样主纹理
sampler alphaCut : register(s1);//alphaCut 提供 alpha 遮罩
float uXTime;//uXTime 推动角向滚动
float uYTime;//uYTime 推动径向滚动
float uRingMult;//uRingMult 决定径向拼几次
float uWidthMult;//uWidthMult 决定角向拼几次

float4 MainPS(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 baseColor = tex2D(alphaCut, coords);
    if (!any(baseColor))
        discard;
    
    float2 vectorFromCenter = coords - 0.5;
    //atan2 的结果落在负 pi 到 pi,加上 0.5 后折回 0 到 1
    float angleFromCenter = atan2(vectorFromCenter.y, vectorFromCenter.x);
    float angle = (angleFromCenter / (2.0 * 3.14159265)) + 0.5;
    float horizontal = frac(angle * uWidthMult + uXTime);
    float dist = distance(coords, 0.5);
    float radial = frac(dist * uRingMult - uYTime);
    float2 polar = float2(horizontal, radial);
    float4 finalColor = tex2D(source, polar);
    float4 OutputColor = finalColor.r * sampleColor * baseColor.a;
    return OutputColor;
}

technique Technique1
{
    pass UCAPolarDistortPass
    {
        PixelShader = compile ps_3_0 MainPS();
    }
}