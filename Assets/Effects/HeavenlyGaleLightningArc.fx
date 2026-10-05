//噪声在 s1,颜色用顶点色,梯形用 TexCoord.z 还原纵向 UV,y 大于 0.5 时噪声反向滚
sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;
matrix uWorldViewProjection;
float4 uShaderSpecificData;

struct VSInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float3 TexCoord : TEXCOORD0;
};

struct VSOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float3 TexCoord : TEXCOORD0;
};

VSOutput VertexFunc(in VSInput input)
{
    VSOutput output = (VSOutput) 0;
    output.Position = mul(input.Position, uWorldViewProjection);
    output.Color = input.Color;
    output.TexCoord = input.TexCoord;
    return output;
}

float4 PixelFunc(VSOutput input) : COLOR0
{
    float4 color = input.Color;
    float2 coords = input.TexCoord.xy;

    coords.y = (coords.y - 0.5) / input.TexCoord.z + 0.5;

    float scrollSign = coords.y > 0.5 ? -1 : 1;
    float distortion = lerp(-1, 1, tex2D(uImage1, coords + float2(0, uTime * scrollSign * 1.81)).r);

    float opacity = pow(sin((coords.y + distortion * 0.15) * 3.141), distortion * 3.95 + 7);

    return color * pow(opacity, 0.25) + opacity;
}

technique Technique1
{
    pass TrailPass
    {
        VertexShader = compile vs_2_0 VertexFunc();
        PixelShader = compile ps_2_0 PixelFunc();
    }
}
