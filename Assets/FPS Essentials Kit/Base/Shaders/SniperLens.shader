// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "FPS Essentials/Sniper Lens" 
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)

		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		_MainTex ("Render Texture", 2D) = "white" {}
		
		_ScopeTex ("Scope (RGBA)", 2D) = "white" {}

		_Opacity ("Opacity", Float) = 40
	}

	CGINCLUDE
	#pragma multi_compile __ VIEWMODEL
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
		Tags { "RenderType"="Opaque" }
		LOD 250
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _ScopeTex;

		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_ScopeTex;
			float3 viewDir;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed _Opacity;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
		// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			fixed4 scope = tex2D(_ScopeTex, IN.uv_ScopeTex);
			c.rgb = lerp(c.rgb, scope.rgb, scope.a);

			fixed dotV = saturate(dot(normalize(IN.viewDir), o.Normal));
			c *= _Color * pow(dotV, _Opacity);

			o.Emission = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
		
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
	FallBack "Diffuse"
}
