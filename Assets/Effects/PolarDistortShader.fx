sampler source : register(s0); // 主纹理
sampler alphaCut : register(s1); // 主纹理
float uXTime; // 游戏时间，用于动画
float uYTime; // 游戏时间，用于动画
float uRingMult; // 纵向的拼贴倍数
float uWidthMult; // 横向的拼贴倍数

float4 MainPS(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 baseColor = tex2D(alphaCut, coords);
    if (!any(baseColor))
        discard;
    
    // 1. 计算到中心的向量，将中心转化为 (0, 0)，也就是对应材质的 (0.5, 0.5)
    float2 vectorFromCenter = coords - 0.5;
    // 2. 修正角度计算：
    // atan2 范围是 [-PI, +PI]
    // (angle / (2.0 * PI)) 范围是 [-0.5, 0.5]
    // (angle / (2.0 * PI)) + 0.5 范围是 [0, 1]
    float angleFromCenter = atan2(vectorFromCenter.y, vectorFromCenter.x);
    float angle = (angleFromCenter / (2.0 * 3.14159265)) + 0.5;
    // *** 应用横向拼贴倍数 uWidthMult ***
    // 将 angle 乘以 uWidthMult，实现横向平铺。
    // 使用 frac() 确保值在 [0, 1] 范围内重复，实现平铺效果。
    // 加上 uTime * 0.1 等可以实现旋转动画。
    float horizontal = frac(angle * uWidthMult + uXTime);
    // 3. 修正距离计算，实现环形扩散：
    // distance() 范围是 [0, 0.707] (在角落)
    // 乘以一个系数（例如 4.0）来增加环的数量
    // - uTime 使其随时间向外扩散
    // frac() 使其在 [0, 1] 范围内重复，形成环
    float dist = distance(coords, 0.5);
    float radial = frac(dist * uRingMult - uYTime); // 4.0 是环的密度，可调
    // 4. 构建新的极坐标 UV
    // angle 是 U 坐标 (环绕)
    // radial 是 V 坐标 (径向)
    float2 polar = float2(horizontal, radial);
    // 5. 采样
    float4 finalColor = tex2D(source, polar);
    // 6. 染色
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