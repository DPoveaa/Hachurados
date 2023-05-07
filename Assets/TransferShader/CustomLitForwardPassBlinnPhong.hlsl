#ifndef LIT_COMMON_INCLUDED
// "#ifndef LIT_COMMON_INCLUDED" is equivalent to "#if !defined(LIT_COMMON_INCLUDED)"
#define LIT_COMMON_INCLUDED

// Pull in URP library functions and our own common functions
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
//#include "LitCommon.hlsl" 
//#include causes the compiler to literally copy and paste the contents of the common file onto this line

// Textures
TEXTURE2D(_ColorMap); SAMPLER(sampler_ColorMap); // RGB = albedo, A = alpha

float4 _ColorMap_ST; // This is automatically set by Unity. Used in TRANSFORM_TEX to apply UV tiling
float4 _ColorTint;
float _Smoothness;
float _Cutoff;

void TestAlphaClip(float4 colorSample) {
#ifdef _ALPHA_CUTOUT
	clip(colorSample.a * _ColorTint.a - _Cutoff);
#endif
}

#endif

struct Attributes {
    float3 positionOS : POSITION; //Position in object space
    float3 normalOS : NORMAL;
    float2 uv : TEXCOORD0; // Material texture UVs

};

struct Interpolators {
    // This value should contain the position in clip space when output from the vertex function
    // It will be transformed into pixel position of the current fragment on the screen
    // when read from the fragment function.
    float4 positionCS : SV_POSITION;

    // The following variables will retain their values from the vertex stage, except the
	// rasterizer will interpolate them between vertices
	float2 uv : TEXCOORD0; // Material textureUVs 
    float3 positionWS : TEXCOORD1;
    float3 normalWS : TEXCOORD2; // the rasterizer will interpolate any field tag with a texcoord semantic.
    
};

// The vertex function. This runs for each vertex on the mesh;
// it must output the position on the screen each vertex should appear at,
// as well as any data fragment function will need.

Interpolators Vertex (Attributes input) {
    Interpolators output;

    // These helper functions, found in URP/ShaderLibrary/ShaderVariablesFunctions.hlsl
    // transform object space values into world and clip space

    VertexPositionInputs posnInputs = GetVertexPositionInputs(input.positionOS);
    VertexNormalInputs normInputs = GetVertexNormalInputs(input.normalOS);

    // Pass position and orientation data to the fragment function
    output.positionCS = posnInputs.positionCS;
    output.uv = TRANSFORM_TEX(input.uv, _ColorMap);
    output.normalWS = normInputs.normalWS;
    output.positionWS = posnInputs.positionWS;

    return output;
}

    // The fragment function; This runs once per fragment, which you can think of as a pixel on the screen
    // It must output the final color of this pixel
float4 Fragment(Interpolators input
#ifdef _DOUBLE_SIDED_NORMALS
	, FRONT_FACE_TYPE frontFace : FRONT_FACE_SEMANTIC
#endif
) : SV_TARGET {
	float2 uv = input.uv;
	float4 colorSample = SAMPLE_TEXTURE2D(_ColorMap, sampler_ColorMap, uv);
	TestAlphaClip(colorSample);

	float3 normalWS = normalize(input.normalWS);
#ifdef _DOUBLE_SIDED_NORMALS
	normalWS *= IS_FRONT_VFACE(frontFace, 1, -1);
#endif

    InputData lightingInput = (InputData)0;
	lightingInput.positionWS = input.positionWS;
	lightingInput.normalWS = normalWS;
	lightingInput.viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
    // convert world space position to a shadow coord
    lightingInput.shadowCoord = TransformWorldToShadowCoord(input.positionWS);

    SurfaceData surfaceInput = (SurfaceData)0;
    surfaceInput.albedo = colorSample.rgb * _ColorTint.rgb;
    surfaceInput.alpha = colorSample.a * _ColorTint.a;
    surfaceInput.specular = 1;
    surfaceInput.smoothness = _Smoothness;

#if UNITY_VERSION >= 202120
	return UniversalFragmentBlinnPhong(lightingInput, surfaceInput);
#else
	return UniversalFragmentBlinnPhong(lightingInput, surfaceInput.albedo, float4(surfaceInput.specular, 1), surfaceInput.smoothness, surfaceInput.emission, surfaceInput.alpha);
#endif
}