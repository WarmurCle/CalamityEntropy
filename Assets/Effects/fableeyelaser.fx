sampler uImage0 : register(s0);

float yofs;
float4 PSFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float c = sin(coords.y * 3.1415);
    return tex2D(uImage0, coords + float2(0, yofs)) * float4(c, c, c, c);
}
technique Technique1
{
    pass fableeyelaser
    {
        PixelShader = compile ps_2_0 PSFunction();
    }
}
