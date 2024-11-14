#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
float2 center;
float size = 1.2;
float b = 560.0 / 300.0;
float r = 0.4;
sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 uv = input.TextureCoordinates;
    float2 direction = center - uv; // 计算指向中心的方向向量
    float distance = length(direction); // 计算到中心的距离
    direction = normalize(direction); // 规范化方向向量
	
    float2 offset = direction * (1.0 - distance * distance) * size;
    offset.x /= b;
    float2 newUV = uv + offset;

    float4 sourceColor = tex2D(SpriteTextureSampler, newUV);

    return sourceColor;
}

technique SpriteDrawing
{
	pass P0
	{
        PixelShader = compile ps_2_0 MainPS();
    }
};