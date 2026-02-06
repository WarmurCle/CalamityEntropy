sampler uImage : register(s0);
float4 color;
float2 texSize;
float4 ShaderFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 colory = tex2D(uImage, frac(coords));
    if(any(colory)){
        return float4(0, 0, 0, 0);
    }
    float xj = 1.0 / texSize.x;
    float yj = 1.0 / texSize.y;
    if (any(tex2D(uImage, coords + float2(-xj, 0))) || any(tex2D(uImage, coords + float2(xj, 0))) || any(tex2D(uImage, coords + float2(0, yj))) || any(tex2D(uImage, coords + float2(0, -yj))) || any(tex2D(uImage, coords + float2(-xj * 2, 0))) || any(tex2D(uImage, coords + float2(xj * 2, 0))) || any(tex2D(uImage, coords + float2(0, yj * 2))) || any(tex2D(uImage, coords + float2(0, -yj * 2))))
        return color * baseColor.a;
    return float4(0, 0, 0, 0);
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 ShaderFunction();
    }
}