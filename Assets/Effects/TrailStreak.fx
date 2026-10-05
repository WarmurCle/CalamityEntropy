//条带在 s1,x 为进度 0头..1尾,y 为横截面,梯形用 TexCoord.z 还原纵向 uv
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

    float streakAlpha = tex2D(uImage1, float2(frac(coords.x - uTime * 2.5), coords.y)).r;

    float edgePower = lerp(3, 10, coords.x);
    float opacity = lerp(pow(sin(coords.y * 3.141), edgePower), streakAlpha, coords.x);

    if (coords.x > 0.7)
        opacity *= pow(1 - (coords.x - 0.7) / 0.3, 6);

    return color * opacity * 1.5;
}

technique Technique1
{
    pass TrailPass
    {
        VertexShader = compile vs_2_0 VertexFunc();
        PixelShader = compile ps_2_0 PixelFunc();
    }
}
