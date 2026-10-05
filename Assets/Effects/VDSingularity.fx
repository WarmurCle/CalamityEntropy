//方形图的 coords 从 0 到 1,中心在 0.5,着色器做极坐标旋流,输出预乘 alpha 并用 AlphaBlend 混合,黑盘不能走 Additive,噪声放在 s1
sampler uImage0 : register(s0);
sampler uImage1 : register(s1);

float uTime;
float3 uColor;//该值是盘体颜色
float3 uColor2;//该值是热边颜色
float uCoreRadius;//该值是视界半径,按归一半径取 0 到 1
float uOpacity;
float uSpin;//该值是旋流的角速度

float4 PixelFunc(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 p = coords - 0.5;
    float r = length(p) * 2.0;
    float ang = atan2(p.y, p.x);

    float twist = ang + uTime * uSpin * (1.5 - r) + 6.0 / (r + 0.35);
    float2 nuv = float2(twist / 6.2831853 + uTime * 0.05, r * 1.2 - uTime * 0.6);
    float n = tex2D(uImage1, frac(nuv)).r;
    float n2 = tex2D(uImage1, frac(nuv * float2(2.0, 0.5) + 0.37)).r;
    float swirl = n * 0.65 + n2 * 0.35;

    float inner = max(uCoreRadius, 0.02);
    float disk = smoothstep(inner, inner + 0.06, r) * (1.0 - smoothstep(0.55, 1.0, r));
    disk *= 0.35 + 0.65 * swirl;
    float doppler = 0.75 + 0.45 * cos(ang - uTime * 0.7);

    float horizon = 1.0 - smoothstep(inner - 0.02, inner, r);
    float rim = pow(saturate(1.0 - abs(r - inner) * 18.0), 2.0);

    float3 col = uColor * disk * doppler + uColor2 * rim * 1.4 + uColor2 * pow(disk, 3.0) * 0.6;
    float a = saturate(disk * 0.9 + rim) * uOpacity;
    //盘体 alpha 再乘 0.55,AlphaBlend 混合下颜色大于 alpha 时会发光
    float4 res = float4(col * a, a * 0.55);
    res = lerp(res, float4(0.0, 0.0, 0.0, uOpacity), horizon);
    return res * baseColor;
}

technique Technique1
{
    pass SingularityPass
    {
        PixelShader = compile ps_3_0 PixelFunc();
    }
}
