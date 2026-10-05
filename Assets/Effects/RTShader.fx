sampler uImage0 : register(s0);//uImage0 绑定 gd.Texture[0]
sampler uImage1 : register(s1);//uImage1 绑定 gd.Texture[1]
//这份着色器沿用 yiyang 的写法,s1 的 UV 按 Offset 滚动
float m;
float n;
float OffsetX = 0;//OffsetX 是 X 方向偏移
float OffsetY = 0;//OffsetY 是 Y 方向偏移
float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 c = tex2D(uImage0,coords);
	float a=max(c.r,max(c.g,c.b));
    if(a>m)//明度超过 m 时这里把像素换成背景
	{
        float4 c1 = tex2D(uImage1, float2((0.5 * coords.x + OffsetX) % 1, (coords.y + OffsetY) % 1));
		return c1;
	}
	else if(abs(a-m)<n)//明度与 m 的差小于 n 时这里把像素换成纯色描边
		return float4(0.02,0.1,0.9,1);
	else
		return c*a;
}

technique Technique1
{
    pass RTShaderPass
    {
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
   
}