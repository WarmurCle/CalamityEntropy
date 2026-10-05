//coords.x 沿轴像素,coords.y 有符号横向像素,不等宽梯形不用 0..1 UV,输出预乘 alpha,Additive,噪声在 s1
sampler uImage1 : register(s1);

float uTime;
float3 uColor;
float3 uColor2;
float uEnvelope;//可见半宽/四边形半宽
float uOpacity;
float uSeed;
float uLengthPx;//轴长,屏幕像素
float uHalfStart;//起点四边形半宽,屏幕像素
float uHalfEnd;//终点四边形半宽,屏幕像素
float uWStart;//起点 1/Scale(Z),平面为 1,越远越大
float uWEnd;//终点 1/Scale(Z),平面为 1,越远越大
float uTile;//噪声沿全长的瓦片数
float uFogStart;//起点雾量,C# 按 VDDepth 预算
float uFogEnd;//终点雾量
float uAlphaStart;//起点深度透明度
float uAlphaEnd;//终点深度透明度
float3 uFogColor;
float2 uCap;//端帽渐隐比例 起,终,按屏幕 t,C# 钳到 >=0.002

float4 PixelFunc(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float t = saturate(coords.x / max(uLengthPx, 1.0));
    float halfQuad = max(lerp(uHalfStart, uHalfEnd, t), 0.001);
    float across = coords.y / halfQuad;
    float v = across * 0.5 + 0.5;

    //1/w 在屏幕上线性,解出沿轴参数 f
    float f = t * uWStart / max((1.0 - t) * uWEnd + t * uWStart, 0.0001);

    float tile = max(uTile, 1.0);
    float n1 = tex2D(uImage1, float2(frac(f * tile * 0.5 - uTime * 1.7 + uSeed), frac(v * 0.9 + uSeed))).r;
    float n2 = tex2D(uImage1, float2(frac(f * tile * 0.23 + uTime * 0.9 + uSeed * 2.0), frac(v * 0.6 + uTime * 0.3))).r;
    float noise = n1 * 0.6 + n2 * 0.4;

    float halfWidth = max(uEnvelope, 0.001) * (0.72 + 0.28 * noise);
    float d = abs(across) / halfWidth;
    float body = saturate(1.0 - d);
    float core = pow(saturate(1.0 - d * 1.8), 4.0);
    float rim = pow(saturate(1.0 - abs(d - 0.85) * 6.0), 2.0) * 0.6;

    float cap = smoothstep(0.0, uCap.x, t) * (1.0 - smoothstep(1.0 - uCap.y, 1.0, t));

    float3 col = uColor * (body * (0.55 + 0.45 * noise)) + uColor2 * core + uColor * rim + core * 0.6;

    float fog = lerp(uFogStart, uFogEnd, f);
    float lum = dot(col, float3(0.333, 0.333, 0.333));
    col = lerp(col, uFogColor * (lum + 0.15), fog);
    float aZ = lerp(uAlphaStart, uAlphaEnd, f);

    float a = saturate(body * 0.85 + core + rim) * cap * uOpacity * aZ;
    return float4(col * a, a) * baseColor;
}

technique Technique1
{
    pass TaperedPass
    {
        PixelShader = compile ps_3_0 PixelFunc();
    }
}
