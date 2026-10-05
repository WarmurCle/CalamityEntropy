//s1 噪声沿 y 置换主贴图 UV,底部按 uFadeRange 淡出
sampler InPutTexture : register(s0);//InPutTexture 是主贴图
sampler uDisplacementSampler : register(s1);//该采样器提供噪声,着色器用它置换 UV

float uTime;
float uIntensity;           //该值决定扭曲有多强
float4 uBaseColor1;          //该值是第一套底色
float4 uTargetColor1;        //该值是第一套目标色
float4 uBaseColor2;          //该值是第二套底色
float4 uTargetColor2;        //该值是第二套目标色
float uColorFactor;         //该值是颜色混合权重,范围 0 到 1
float2 uNoiseScale;         //该值缩放噪声的 UV
float uFadeRange = 0.3f;    //该值决定底部淡出有多长,范围 0 到 1
bool UseColor;              //该值为真时给结果染色

float4 DisplacementFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 uBaseColor = lerp(uBaseColor1, uBaseColor2, coords.y * 2);
    float4 uTargetColor = lerp(uTargetColor1, uTargetColor2, coords.y * 2);

    float2 noiseUV = coords * uNoiseScale;
    noiseUV += float2(0, -uTime * 0.1);

    float displacement = tex2D(uDisplacementSampler, noiseUV).r;
    float2 offset = float2(0, (displacement - 0.5) * 2 * uIntensity);
    float2 displacedUV = saturate(coords + offset);

    float4 originalColor = tex2D(InPutTexture, displacedUV);
    float fadeFactor = 1.0f;
    if(coords.y > (1.0f - uFadeRange))
    {
        fadeFactor = lerp(1.0f,0.0f,(coords.y - (1.0f - uFadeRange)) / uFadeRange);
    }
    float4 finalColor = originalColor;
    if (UseColor)
    {
        float4 mixedColor = lerp(uBaseColor, uTargetColor, uColorFactor);
        finalColor = originalColor * mixedColor;
        finalColor.a = originalColor.a * lerp(uBaseColor.a, uTargetColor.a, uColorFactor);
    }

    return finalColor;
}

technique Technique1
{
    pass LPADisplacementPass
    {
        PixelShader = compile ps_2_0 DisplacementFunction();
    }
}