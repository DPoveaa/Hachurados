#ifndef LIT_COMMON_INCLUDED
#define LIT_COMMON_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

TEXTURE2D(_ColorMap); SAMPLER(sampler_ColorMap);

float4 _ColorMap_ST;
float4 _ColorTint;
float _Cutoff;
float _Smoothness;

void TestAlphaClip(float4 colorSample) {
#ifdef _ALPHA_CUTOUT
	clip(colorSample.a * _ColorTint.a - _Cutoff);
#endif
}

#endif