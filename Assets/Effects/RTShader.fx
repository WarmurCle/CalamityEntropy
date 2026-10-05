sampler uImage0 : register(s0);//gd.Texture[0]
sampler uImage1 : register(s1);//gd.Texture[1]
//抄自 yiyang,s1 按 Offset 滚动
float m;
float n;
float OffsetX = 0;//X 偏移
float OffsetY = 0;//Y 偏移
float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 c = tex2D(uImage0,coords);
	float a=max(c.r,max(c.g,c.b));
    if(a>m)//明度超过 m 换成背景
	{
        float4 c1 = tex2D(uImage1, float2((0.5 * coords.x + OffsetX) % 1, (coords.y + OffsetY) % 1));
		return c1;
	}
	else if(abs(a-m)<n)//与 m 差小于 n 换成纯色描边
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