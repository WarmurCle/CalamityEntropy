sampler uImage : register(s0);
float4 color1;
float4 color2;
float alpha;
float4 EffectFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 colory = tex2D(uImage, coords);
    float4 color3 = float4(0, 0, 0, 0);
    if(colory.r > 0.8){
        float z = (colory.r - 0.8) * 3.5;
        color3 = float4(z, z, z, 1);
    }
    return (lerp(color1, color2, colory.r) + color3) * float4(1, 1, 1, colory.r * alpha);
}

technique Technique1
{
    pass EffectPass
    {
        PixelShader = compile ps_2_0 EffectFunction();
    }
}