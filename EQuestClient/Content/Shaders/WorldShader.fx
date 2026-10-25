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
	
	bool FogEnabled;
	float4 FogColor;
	float4 FogVector;
	float4 AlphaTest;
};

sampler Texture;

struct VSInput
{
	float4 pos: POSITION0;
	float2 texCoord: TEXCOORD0;
};

struct PSInput
{
	float4 pos: SV_Position;
	float2 texCoord: TEXCOORD0;
	float fogFactor: TEXCOORD1;
};

float ComputeFogFactor(float4 position)
{
	return saturate(dot(position, FogVector));
}

void ApplyFog(inout float4 color, float fogFactor)
{
	color.rgb = lerp(color.rgb, FogColor * color.a, fogFactor);
}

PSInput VertexShader0(VSInput input)
{
	PSInput output = (PSInput)0;
	
	input.pos.w = 1.0f;
	output.pos = input.pos;
	output.pos = mul(output.pos, World);
	output.pos = mul(output.pos, View);
	output.pos = mul(output.pos, Projection);
	output.texCoord = input.texCoord;

	if (FogEnabled)
		output.fogFactor = ComputeFogFactor(input.pos);

	return output;
}

float4 PixelShader0(PSInput input) : SV_Target
{
	float4 texColor = tex2D(Texture, input.texCoord);

	clip((texColor.a < AlphaTest.x) ? AlphaTest.z : AlphaTest.w);

	if (FogEnabled)
		ApplyFog(texColor, input.fogFactor);

	return texColor;
}

technique Technique0
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL VertexShader0();
		PixelShader = compile PS_SHADERMODEL PixelShader0();
	}
};