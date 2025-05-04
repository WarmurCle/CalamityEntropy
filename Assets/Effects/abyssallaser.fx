sampler uImage0 : register(s0);

float yofs;

float4 PSFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float px = 1 - cos(coords.y * 3.1415 - 3.1415 * 0.5);
    float pj = px;
    if(coords.y < 0.5){
        pj = -px;
    }
    float c = sin(coords.y * 3.1415);
    float4 clr = tex2D(uImage0, float2(coords.x, coords.y * 0.1 + yofs + pj * 0.6));
    return (clr * 1.4 + float4(c * c, c * c, c * c, 1) * 0.2) * float4(c, c, c, c) * baseColor;
}
technique Technique1
{
    pass fableeyelaser
    {
        PixelShader = compile ps_2_0 PSFunction();
    }
}
