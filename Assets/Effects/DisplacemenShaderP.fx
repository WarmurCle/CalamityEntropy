//s1 噪声沿 y 置换主贴图 UV,底部按 uFadeRange 淡出
sampler InPutTexture : register(s0);//主贴图
sampler uDisplacementSampler : register(s1);//噪声置换

float uTime;
float uIntensity;           //扭曲强度
float4 uBaseColor1;          //底色
float4 uTargetColor1;        //目标色
float4 uBaseColor2;          //底色 2
float4 uTargetColor2;        //目标色 2
float uColorFactor;         //颜色权重 0..1
float2 uNoiseScale;         //噪声 UV 缩放
float uFadeRange = 0.3f;    //底部淡出 0..1
bool UseColor;              //染色开关

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