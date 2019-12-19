Shader "FPS Essentials/Viewmodel (Specular setup)"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo", 2D) = "white" {}
		
		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
		_GlossMapScale("Smoothness Factor", Range(0.0, 1.0)) = 1.0
		[Enum(Specular Alpha,0,Albedo Alpha,1)] _SmoothnessTextureChannel ("Smoothness texture channel", Float) = 0

		_SpecColor("Specular", Color) = (0.2,0.2,0.2)
		_SpecGlossMap("Specular", 2D) = "white" {}
		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[ToggleOff] _GlossyReflections("Glossy Reflections", Float) = 1.0

		_BumpScale("Scale", Float) = 1.0
		_BumpMap("Normal Map", 2D) = "bump" {}

		_Parallax ("Height Scale", Range (0.005, 0.08)) = 0.02
		_ParallaxMap ("Height Map", 2D) = "black" {}

		_OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
		_OcclusionMap("Occlusion", 2D) = "white" {}

		_EmissionColor("Color", Color) = (0,0,0)
		_EmissionMap("Emission", 2D) = "white" {}
		
		_DetailMask("Detail Mask", 2D) = "white" {}

		_DetailAlbedoMap("Detail Albedo x2", 2D) = "grey" {}
		_DetailNormalMapScale("Scale", Float) = 1.0
		_DetailNormalMap("Normal Map", 2D) = "bump" {}

		[Enum(UV0,0,UV1,1)] _UVSec ("UV Set for secondary textures", Float) = 0

		// Blending state
		[HideInInspector] _Mode ("__mode", Float) = 0.0
		[HideInInspector] _SrcBlend ("__src", Float) = 1.0
		[HideInInspector] _DstBlend ("__dst", Float) = 0.0
		[HideInInspector] _ZWrite ("__zw", Float) = 1.0
	}

	CGINCLUDE
	#pragma multi_compile __ VIEWMODEL
	#define UNITY_SETUP_BRDF_INPUT SpecularSetup

	#include "UnityCG.cginc"

    // Object rendering things
	#if defined(USING_STEREO_MATRICES)
		float4x4 _StereoNonJitteredVP[2];
		float4x4 _StereoPreviousVP[2];
	#else
		float4x4 _NonJitteredVP;
		float4x4 _PreviousVP;
	#endif

    float4x4 _PreviousM;
    bool _HasLastPositionData;
    bool _ForceNoMotion;
    float _MotionVectorDepthBias;

    struct MotionVectorData
    {
        float4 transferPos : TEXCOORD0;
        float4 transferPosOld : TEXCOORD1;
        float4 pos : SV_POSITION;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    struct MotionVertexInput
    {
        float4 vertex : POSITION;
        float3 oldPos : NORMAL;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    MotionVectorData VertMotionVectors(MotionVertexInput v)
    {
        MotionVectorData o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
        o.pos = UnityObjectToClipPos(v.vertex);

        // this works around an issue with dynamic batching
        // potentially remove in 5.4 when we use instancing
		#if defined(UNITY_REVERSED_Z)
			o.pos.z -= _MotionVectorDepthBias * o.pos.w;
		#else
			o.pos.z += _MotionVectorDepthBias * o.pos.w;
		#endif

		#if defined(USING_STEREO_MATRICES)
			o.transferPos = mul(_StereoNonJitteredVP[unity_StereoEyeIndex], mul(unity_ObjectToWorld, v.vertex));
			o.transferPosOld = mul(_StereoPreviousVP[unity_StereoEyeIndex], mul(_PreviousM, _HasLastPositionData ? float4(v.oldPos, 1) : v.vertex));
		#else
			o.transferPos = mul(_NonJitteredVP, mul(unity_ObjectToWorld, v.vertex));
			o.transferPosOld = mul(_PreviousVP, mul(_PreviousM, _HasLastPositionData ? float4(v.oldPos, 1) : v.vertex));
		#endif

        return o;
    }

    half4 FragMotionVectors(MotionVectorData i) : SV_Target
    {
        float3 hPos = (i.transferPos.xyz / i.transferPos.w);
        float3 hPosOld = (i.transferPosOld.xyz / i.transferPosOld.w);

        // V is the viewport position at this pixel in the range 0 to 1.
        float2 vPos = (hPos.xy + 1.0f) / 2.0f;
        float2 vPosOld = (hPosOld.xy + 1.0f) / 2.0f;

		#if UNITY_UV_STARTS_AT_TOP
			vPos.y = 1.0 - vPos.y;
			vPosOld.y = 1.0 - vPosOld.y;
		#endif

        half2 uvDiff = vPos - vPosOld;
        return lerp(half4(uvDiff, 0, 1), 0, (half)_ForceNoMotion);
    }

    //Camera rendering things
    UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

    struct CamMotionVectors
    {
        float4 pos : SV_POSITION;
        float2 uv : TEXCOORD0;
        float3 ray : TEXCOORD1;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    struct CamMotionVectorsInput
    {
        float4 vertex : POSITION;
        float3 normal : NORMAL;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    CamMotionVectors VertMotionVectorsCamera(CamMotionVectorsInput v)
    {
        CamMotionVectors o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
        o.pos = UnityObjectToClipPos(v.vertex);

		#ifdef UNITY_HALF_TEXEL_OFFSET
			o.pos.xy += (_ScreenParams.zw - 1.0) * float2(-1, 1) * o.pos.w;
		#endif
        o.uv = ComputeScreenPos(o.pos);
        // we know we are rendering a quad,
        // and the normal passed from C++ is the raw ray.
        o.ray = v.normal;
        return o;
    }

    inline half2 CalculateMotion(float rawDepth, float2 inUV, float3 inRay)
    {
        float depth = Linear01Depth(rawDepth);
        float3 ray = inRay * (_ProjectionParams.z / inRay.z);
        float3 vPos = ray * depth;
        float4 worldPos = mul(unity_CameraToWorld, float4(vPos, 1.0));

		#if defined(USING_STEREO_MATRICES)
			float4 prevClipPos = mul(_StereoPreviousVP[unity_StereoEyeIndex], worldPos);
			float4 curClipPos = mul(_StereoNonJitteredVP[unity_StereoEyeIndex], worldPos);
		#else
			float4 prevClipPos = mul(_PreviousVP, worldPos);
			float4 curClipPos = mul(_NonJitteredVP, worldPos);
		#endif

        float2 prevHPos = prevClipPos.xy / prevClipPos.w;
        float2 curHPos = curClipPos.xy / curClipPos.w;

        // V is the viewport position at this pixel in the range 0 to 1.
        float2 vPosPrev = (prevHPos.xy + 1.0f) / 2.0f;
        float2 vPosCur = (curHPos.xy + 1.0f) / 2.0f;

		#if UNITY_UV_STARTS_AT_TOP
			vPosPrev.y = 1.0 - vPosPrev.y;
			vPosCur.y = 1.0 - vPosCur.y;
		#endif

        return vPosCur - vPosPrev;
    }

    half4 FragMotionVectorsCamera(CamMotionVectors i) : SV_Target
    {
        float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
        return half4(CalculateMotion(depth, i.uv, i.ray), 0, 1);
    }

    half4 FragMotionVectorsCameraWithDepth(CamMotionVectors i, out float outDepth : SV_Depth) : SV_Target
    {
        float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
        outDepth = depth;
        return half4(CalculateMotion(depth, i.uv, i.ray), 0, 1);
    }
	ENDCG

	SubShader
	{
		Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
		LOD 300
	

		// ------------------------------------------------------------------
		//  Base forward pass (directional light, emission, lightmaps, ...)
		Pass
		{
			Name "FORWARD" 
			Tags { "LightMode" = "ForwardBase" }

			Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]

			CGPROGRAM
			#pragma target 3.0

			// -------------------------------------

			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma shader_feature _EMISSION
			#pragma shader_feature _SPECGLOSSMAP
			#pragma shader_feature ___ _DETAIL_MULX2
			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
			#pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
			#pragma shader_feature _ _GLOSSYREFLECTIONS_OFF
			#pragma shader_feature _PARALLAXMAP

			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog
			#pragma multi_compile_instancing

			#pragma vertex vertBase
			#pragma fragment fragBase
			#include "UnityStandardCoreForward.cginc"

			ENDCG
		}
		// ------------------------------------------------------------------
		//  Additive forward pass (one light per pass)
		Pass
		{
			Name "FORWARD_DELTA"
			Tags { "LightMode" = "ForwardAdd" }
			Blend [_SrcBlend] One
			Fog { Color (0,0,0,0) } // in additive pass fog should be black
			ZWrite Off
			ZTest LEqual

			CGPROGRAM
			#pragma target 3.0

			// -------------------------------------

			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma shader_feature _SPECGLOSSMAP
			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
			#pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
			#pragma shader_feature ___ _DETAIL_MULX2
			#pragma shader_feature _PARALLAXMAP

			#pragma multi_compile_fwdadd_fullshadows
			#pragma multi_compile_fog

			#pragma vertex vertAdd
			#pragma fragment fragAdd
			#include "UnityStandardCoreForward.cginc"

			ENDCG
		}
		// ------------------------------------------------------------------
		//  Shadow rendering pass
		Pass {
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }

			ZWrite On ZTest LEqual

			CGPROGRAM
			#pragma target 3.0

			// -------------------------------------


			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma shader_feature _SPECGLOSSMAP
			#pragma shader_feature _PARALLAXMAP
			#pragma multi_compile_shadowcaster
			#pragma multi_compile_instancing

			#pragma vertex vertShadowCaster
			#pragma fragment fragShadowCaster

			#include "UnityStandardShadow.cginc"

			ENDCG
		}
		// ------------------------------------------------------------------
		//  Deferred pass
		Pass
		{
			Name "DEFERRED"
			Tags { "LightMode" = "Deferred" }

			CGPROGRAM
			#pragma target 3.0
			#pragma exclude_renderers nomrt


			// -------------------------------------

			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma shader_feature _EMISSION
			#pragma shader_feature _SPECGLOSSMAP
			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
			#pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
			#pragma shader_feature ___ _DETAIL_MULX2
			#pragma shader_feature _PARALLAXMAP

			#pragma multi_compile_prepassfinal
			#pragma multi_compile_instancing

			#pragma vertex vertDeferred
			#pragma fragment fragDeferred

			#include "UnityStandardCore.cginc"

			ENDCG
		}

		// ------------------------------------------------------------------
		// Extracts information for lightmapping, GI (emission, albedo, ...)
		// This pass it not used during regular rendering.
		Pass
		{
			Name "META" 
			Tags { "LightMode"="Meta" }

			Cull Off

			CGPROGRAM
			#pragma vertex vert_meta
			#pragma fragment frag_meta

			#pragma shader_feature _EMISSION
			#pragma shader_feature _SPECGLOSSMAP
			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
			#pragma shader_feature ___ _DETAIL_MULX2
			#pragma shader_feature EDITOR_VISUALIZATION

			#include "UnityStandardMeta.cginc"
			ENDCG
		}

		// 0 - Motion vectors
        Pass
        {
            Tags{ "LightMode" = "MotionVectors" }

            ZTest LEqual
            Cull Back
            ZWrite Off

            CGPROGRAM
            #pragma vertex VertMotionVectors
            #pragma fragment FragMotionVectors
            ENDCG
        }

		// Removed to avoid errors with material preview in editor, still studying what is happening

		/*
        // 1 - Camera motion vectors
        Pass
        {
            ZTest Always
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex VertMotionVectorsCamera
            #pragma fragment FragMotionVectorsCamera
            ENDCG
        }

        // 2 - Camera motion vectors (With depth (msaa / no render texture))
        Pass
        {
            ZTest Always
            Cull Off
            ZWrite On

            CGPROGRAM
            #pragma vertex VertMotionVectorsCamera
            #pragma fragment FragMotionVectorsCameraWithDepth
            ENDCG
        }*/
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
		LOD 150

		// ------------------------------------------------------------------
		//  Base forward pass (directional light, emission, lightmaps, ...)
		Pass
		{
			Name "FORWARD" 
			Tags { "LightMode" = "ForwardBase" }

			Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]

			CGPROGRAM
			#pragma target 2.0
			
			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma shader_feature _EMISSION 
			#pragma shader_feature _SPECGLOSSMAP
			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
			#pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
			#pragma shader_feature _ _GLOSSYREFLECTIONS_OFF
			#pragma shader_feature ___ _DETAIL_MULX2

			#pragma skip_variants SHADOWS_SOFT DYNAMICLIGHTMAP_ON DIRLIGHTMAP_COMBINED
			
			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog

			#pragma vertex vertBase
			#pragma fragment fragBase
			#include "UnityStandardCoreForward.cginc"

			ENDCG
		}
		// ------------------------------------------------------------------
		//  Additive forward pass (one light per pass)
		Pass
		{
			Name "FORWARD_DELTA"
			Tags { "LightMode" = "ForwardAdd" }
			Blend [_SrcBlend] One
			Fog { Color (0,0,0,0) } // in additive pass fog should be black
			ZWrite Off
			ZTest LEqual
			
			CGPROGRAM
			#pragma target 2.0

			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma shader_feature _SPECGLOSSMAP
			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
			#pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
			#pragma shader_feature ___ _DETAIL_MULX2
			#pragma skip_variants SHADOWS_SOFT
			
			#pragma multi_compile_fwdadd_fullshadows
			#pragma multi_compile_fog
			
			#pragma vertex vertAdd
			#pragma fragment fragAdd
			#include "UnityStandardCoreForward.cginc"

			ENDCG
		}
		// ------------------------------------------------------------------
		//  Shadow rendering pass
		Pass {
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }
			
			ZWrite On ZTest LEqual

			CGPROGRAM
			#pragma target 2.0

			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma shader_feature _SPECGLOSSMAP
			#pragma skip_variants SHADOWS_SOFT
			#pragma multi_compile_shadowcaster

			#pragma vertex vertShadowCaster
			#pragma fragment fragShadowCaster

			#include "UnityStandardShadow.cginc"

			ENDCG
		}
		// ------------------------------------------------------------------
		// Extracts information for lightmapping, GI (emission, albedo, ...)
		// This pass it not used during regular rendering.
		Pass
		{
			Name "META" 
			Tags { "LightMode"="Meta" }

			Cull Off

			CGPROGRAM
			#pragma vertex vert_meta
			#pragma fragment frag_meta

			#pragma shader_feature _EMISSION
			#pragma shader_feature _SPECGLOSSMAP
			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
			#pragma shader_feature ___ _DETAIL_MULX2
			#pragma shader_feature EDITOR_VISUALIZATION

			#include "UnityStandardMeta.cginc"
			ENDCG
		}
	}

	FallBack "VertexLit"
	CustomEditor "StandardShaderGUI"
}
