//圆形 UV 朝边缘挤出,噪声 wrap,沿横轴滚,SpriteBatch Immediate
float time;
float blowUpPower;   //噪声球面化幂次
float blowUpSize;    //球面化挤出强度
float3 shieldColor;
float shieldOpacity;
float3 shieldEdgeColor;
float shieldEdgeBlendStrenght;//拼写来自原调用契约,故意保留
float noiseScale;
float resolution;

//SpriteBatch 当前贴图,s0,wrap
texture sampleTexture;
sampler2D NoiseSampler = sampler_state { texture = <sampleTexture>; magfilter = LINEAR; minfilter = LINEAR; mipfilter = LINEAR; AddressU = wrap; AddressV = wrap; };

float4 PixelFunc(float2 uv : TEXCOORD) : COLOR
{
    float distFromCenter = length(uv - float2(0.5, 0.5)) * 2;
    if (distFromCenter > 1)
        return float4(0, 0, 0, 0);

    float bulgeX = pow(abs(uv.x - 0.5) * 2, blowUpPower);
    float bulgeY = pow(abs(uv.y - 0.5) * 2, blowUpPower);
    float2 sphereUV = float2(
        uv.x * (1 + bulgeY * blowUpSize) - bulgeY * blowUpSize * 0.5,
        uv.y * (1 + bulgeX * blowUpSize) - bulgeX * blowUpSize * 0.5);

    sphereUV *= noiseScale;
    sphereUV.x = (sphereUV.x + time) % 1;

    float4 noiseColor = tex2D(NoiseSampler, sphereUV);

    noiseColor += pow(distFromCenter, 6);

    if (distFromCenter > 0.95)
        noiseColor *= 1 - (distFromCenter - 0.95) / 0.05;

    return noiseColor * float4(lerp(shieldColor, shieldEdgeColor, pow(distFromCenter, shieldEdgeBlendStrenght)), shieldOpacity);
}

technique Technique1
{
    pass ShieldPass
    {
        PixelShader = compile ps_2_0 PixelFunc();
    }
}
