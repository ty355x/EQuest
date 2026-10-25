#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

cbuffer CBufferPerObject
{
	float4x4 World;
	float4x4 View;
	float4x4 Projection;
	float4 SurfaceColor;
};

struct VSInput
{
	float4 pos: POSITION0;
	float2 texCoord: TEXCOORD0;
};

struct PSInput
{
	float4 pos: SV_Position;
	float2 texCoord: TEXCOORD0;
};

PSInput VertexShader0(VSInput input)
{
	PSInput output = (PSInput)0;

	input.pos.w = 1.0f;
	output.pos = input.pos;
	output.pos = mul(output.pos, World);
	output.pos = mul(output.pos, View);
	output.pos = mul(output.pos, Projection);
	output.texCoord = input.texCoord;

	return output;
}

float4 PixelShader0(PSInput input) : SV_Target
{
	return SurfaceColor;
}

technique Technique0
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL VertexShader0();
		PixelShader = compile PS_SHADERMODEL PixelShader0();
	}
};