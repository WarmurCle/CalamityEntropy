//极坐标 UV,角向 uWidthMult,径向 uRingMult,s1 作 alpha
sampler source : register(s0);//主纹理
sampler alphaCut : register(s1);//alpha 遮罩
float uXTime;//角向滚动
float uYTime;//径向滚动
float uRingMult;//径向拼贴
float uWidthMult;//角向拼贴

float4 MainPS(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 baseColor = tex2D(alphaCut, coords);
    if (!any(baseColor))
        discard;
    
    float2 vectorFromCenter = coords - 0.5;
    //atan2 是 -pi..pi,加 0.5 折回 0..1
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