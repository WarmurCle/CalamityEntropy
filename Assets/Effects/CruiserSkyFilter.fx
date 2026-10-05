//噪声位移画面 UV,竖向带状衰减,uOpacity 为强度乘滤镜淡入
sampler uImage0 : register(s0);   //捕获的画面
sampler uImage1 : register(s1);   //噪声 VoidBack

float uTime;          //原版 GlobalTimeWrappedHourly
float uOpacity;       //强度乘滤镜淡入
float2 uScreenOffCE;  //镜头视差
float2 uCoordMultCE;  //1/GameViewMatrix.Zoom
float uBandCenterCE;  //扭曲带中心,屏高比例,重力翻转时翻到下侧

float4 PixelFunc(float2 uv : TEXCOORD0) : COLOR0
{
    float strengthMult = 0.28 * uOpacity;
    //旧遮罩近乎全屏,这里用竖向软带
    float dy = (uv.y - uBandCenterCE) * 1.6;
    float band = exp2(-dy * dy);
    float strength = strengthMult * band;

    float t = uTime * 0.03;
    float n1 = tex2D(uImage1, frac(uv * uCoordMultCE + float2(t + 0.4, t + 0.7) + uScreenOffCE)).r;
    float n2 = tex2D(uImage1, frac(uv * uCoordMultCE + float2(-t + 0.3, -t + 0.5) + uScreenOffCE)).r;
    float2 offset = float2(n1 - 0.5, n2 - 0.5) * strength * 0.2;

    float4 color = tex2D(uImage0, uv + offset);
    float cc = (color.r + color.g + color.b - 1.12) * 3.2;//沿用旧 fscreenCr
    color.rgb *= 1 + cc * strengthMult;
    return color;
}

technique Technique1
{
    pass CruiserSkyPass
    {
        PixelShader = compile ps_3_0 PixelFunc();
    }
}
