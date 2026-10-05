//噪声在 s1,亮度条带在 s2,梯形用 TexCoord.z 还原纵向 uv
sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
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
float2 uImageSize2;
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

    float bloom = pow(sin(coords.y * 3.141), 5.6);

    float noise = tex2D(uImage1, coords * 3 - float2(uTime * 2.44, 0));

    float brightStreak = tex2D(uImage2, coords * float2(2, 1) - float2(uTime * 1.61, 0)) + noise * bloom;

    float4 energyColor = float4(lerp(uColor, uSecondaryColor, noise), 1);

    return (energyColor * bloom + brightStreak * bloom) * color.a * pow(1 - coords.x, 1.6);
}

technique Technique1
{
    pass PiercePass
    {
        VertexShader = compile vs_2_0 VertexFunc();
        PixelShader = compile ps_2_0 PixelFunc();
    }
}
