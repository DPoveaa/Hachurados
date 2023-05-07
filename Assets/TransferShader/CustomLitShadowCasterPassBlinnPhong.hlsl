
#ifndef LIT_SHADOW_CASTER_PASS_INCLUDED
#define LIT_SHADOW_CASTER_PASS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "LitCommonBlinnPhong.hlsl"

struct Attributes {
    float3 positionOS : POSITION;
    float3 normalOS : NORMAL;
#ifdef _ALPHA_CUTOUT
	float2 uv : TEXCOORD0;
#endif
};

struct Interpolators {
    float4 positionCS : SV_POSITION;
#ifdef _ALPHA_CUTOUT
	float2 uv : TEXCOORD0;
#endif
};

float3 FlipNormalBasedOnViewDir(float3 normalWS, float3 positionWS) {
	float3 viewDirWS = GetWorldSpaceNormalizeViewDir(positionWS);
	return normalWS * (dot(normalWS, viewDirWS) < 0 ? -1 : 1);
}

//function to offset clip space position
float3 _LightDirection;

float4 GetShadowCasterPositionCS(float3 positionWS, float3 normalWS) {
	float3 lightDirectionWS = _LightDirection;
#ifdef _DOUBLE_SIDED_NORMALS
    //Modify the clip space calculation to call FlipNormalBasedOnViewDir.
	normalWS = FlipNormalBasedOnViewDir(normalWS, positionWS);
#endif
    
    float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));

#if UNITY_REVERSED_Z
	positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
#else
	positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
#endif
	return positionCS;
}

Interpolators Vertex (Attributes input) {
    Interpolators output;

    VertexPositionInputs posnInputs = GetVertexPositionInputs(input.positionOS);
    VertexNormalInputs normInputs = GetVertexNormalInputs(input.normalOS);
    
    output.positionCS = GetShadowCasterPositionCS(posnInputs.positionWS, normInputs.normalWS);
#ifdef _ALPHA_CUTOUT
	output.uv = TRANSFORM_TEX(input.uv, _ColorMap);
#endif
    return output;
}

    // The fragment function; This runs once per fragment, which you can think of as a pixel on the screen
    // It must output the final color of this pixel
float4 Fragment(Interpolators input) : SV_TARGET {
#ifdef _ALPHA_CUTOUT
	float2 uv = input.uv;
	float4 colorSample = SAMPLE_TEXTURE2D(_ColorMap, sampler_ColorMap, uv);
	TestAlphaClip(colorSample);
#endif
    return 0;
}

#endif