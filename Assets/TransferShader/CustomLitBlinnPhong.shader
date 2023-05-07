Shader "DeliveryBox/CustomLitBlinnPhong"
{
    // Properties are options set per material, exposed by the material inspector
    Properties {
        [Header(Surface options)] // Creates a text header
        // [MainColor] allows Material.color to use the correct property
        [MainTexture] _ColorMap("Color", 2D) = "white" {}
        [MainColor] _ColorTint("Tint", Color) = (1, 1, 1, 1)
        _Cutoff("Alpha cutout threshold", Range(0,1)) = 0.5
        _Smoothness("Smoothness", Range(0, 1)) = 0.5 //for blinnphong, substitute Range(0, 1) for Float

        [HideInInspector] _SourceBlend("Source blend", Float) = 0
        [HideInInspector] _DestBlend("Destination blend", Float) = 0
        [HideInInspector] _ZWrite("ZWrite", Float) = 0

        [HideInInspector] _SurfaceType("Surface Type", Float) = 0
        [HideInInspector] _FaceRenderingMode("Face rendering type", Float) = 0
    }
        
    // Subshaders allow for different behaviour and options for different pipelines and platforms
    SubShader
    {
        // These tags are shared by all passes in this sub shader
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque"}

        //Each pass has it's own vertex and fragment function and shader variant keywords
        Pass
        {
            Name "ForwardLit" //for debugging
            Tags{"LightMode" = "UniversalForward"} //Pass specific tags.
            // "UniversalForward" tells Unity this is the main lighting pass of this shader

            //Blend SrcAlpha OneMinusSrcAlpha //Determines how the rasterizer combines fragment functions outputs
            //with colors already presented on the screen.
            //ZWrite Off

            Blend[_SourceBlend][_DestBlend]
            ZWrite[_ZWrite]
            Cull[_Cull]

            HLSLPROGRAM //Begin HLSL code

            #define _SPECULAR_COLOR // for blinnphong shader
            #pragma shader_feature_local _ALPHA_CUTOUT
            #pragma shader_feature_local _DOUBLE_SIDED_NORMALS

#if UNITY_VERSION >= 202120
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
#else
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
#endif
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            //Register our programmable stage functions
            #pragma vertex Vertex
            #pragma fragment Fragment

            //Include our code file
            #include "CustomLitForwardPassBlinnPhong.hlsl"

            ENDHLSL
        }

        Pass
        {
            Name "ForwardLit" //for debugging
            Tags{"LightMode" = "ShadowCaster"}

            ColorMask 0 //Turn off color to the shadow caster
            Cull[_Cull]

            HLSLPROGRAM 

            #pragma shader_feature_local _ALPHA_CUTOUT
            #pragma shader_feature_local _DOUBLE_SIDED_NORMALS

            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "CustomLitShadowCasterPassBlinnPhong.hlsl"

            ENDHLSL
        }
    }

    CustomEditor "LitCustomInspector"
}