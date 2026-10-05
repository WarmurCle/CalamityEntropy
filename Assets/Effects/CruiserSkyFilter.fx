//噪声推动画面的 UV,强度沿竖向带衰减,uOpacity 是强度乘上滤镜淡入
sampler uImage0 : register(s0);   //uImage0 是捕获下来的画面
sampler uImage1 : register(s1);   //uImage1 是噪声 VoidBack

float uTime;          //该值取自原版的 GlobalTimeWrappedHourly
float uOpacity;       //该值是强度乘上滤镜淡入
float2 uScreenOffCE;  //该值是镜头视差
float2 uCoordMultCE;  //该值是 1 除以 GameViewMatrix.Zoom
float uBandCenterCE;  //该值是扭曲带中心,按屏高比例,重力翻转时翻到下侧

float4 PixelFunc(float2 uv : TEXCOORD0) : COLOR0
{
    float strengthMult = 0.28 * uOpacity;
    //旧遮罩几乎铺满全屏,这里改成竖向软带
    float dy = (uv.y - uBandCenterCE) * 1.6;
    float band = exp2(-dy * dy);
    float strength = strengthMult * band;

    float t = uTime * 0.03;
    float n1 = tex2D(uImage1, frac(uv * uCoordMultCE + float2(t + 0.4, t + 0.7) + uScreenOffCE)).r;
    float n2 = tex2D(uImage1, frac(uv * uCoordMultCE + float2(-t + 0.3, -t + 0.5) + uScreenOffCE)).r;
    float2 offset = float2(n1 - 0.5, n2 - 0.5) * strength * 0.2;

    float4 color = tex2D(uImage0, uv + offset);
    float cc = (color.r + color.g + color.b - 1.12) * 3.2;//cc 沿用旧的 fscreenCr 算法
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
