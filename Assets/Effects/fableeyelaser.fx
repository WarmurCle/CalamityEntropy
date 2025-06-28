sampler uImage0 : register(s0);

float yofs;

float4 PSFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float pj = coords.y + sin(coords.y * 3.1415 - 3.1415 * 0.5) * 0.5 * 0.4;
    //pj = coords.y + (coords.y - pj);
    float c = sin(coords.y * 3.1415);
    float4 clr = tex2D(uImage0, float2(coords.x * 2.4, frac(yofs + pj)));
    if(clr.r > 0.8)
    {
        float z = (clr.r - 0.8) * 1.6;
        clr += float4(z, z, z, z);
    }
    float c1 = c * c * c * 0.25;
    clr += float4(c1, c1, c1, c1);
    float c2 = c;
    return clr * float4(c2, c2, c2, c2) * baseColor;
}
technique Technique1
{
    pass fableeyelaser
    {
        PixelShader = compile ps_2_0 PSFunction();
    }
}
