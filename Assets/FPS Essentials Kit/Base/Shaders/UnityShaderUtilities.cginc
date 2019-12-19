#ifndef UNITY_SHADER_UTILITIES_INCLUDED
#define UNITY_SHADER_UTILITIES_INCLUDED

#include "UnityShaderVariables.cginc"

float4x4 CUSTOM_MATRIX_P;
 
#if defined(VIEWMODEL)
    // Tranforms position from object to homogenous space
    inline float4 UnityObjectToClipPos(in float3 pos)
    {
        float4x4 mvp = mul(mul(CUSTOM_MATRIX_P, UNITY_MATRIX_V), unity_ObjectToWorld);

        float4 result = mul(mvp, float4(pos, 1.0));

        #if SHADER_API_D3D11
            result.y *= -1;
        #endif

        result.z *= 0.5;

        return result;
    }
#else
    // Tranforms position from object to homogenous space
    inline float4 UnityObjectToClipPos(in float3 pos)
    {
        // More efficient than computing M*VP matrix product
        return mul(UNITY_MATRIX_VP, mul(unity_ObjectToWorld, float4(pos, 1.0)));
    }
#endif

inline float4 UnityObjectToClipPos(float4 pos) // overload for float4; avoids "implicit truncation" warning for existing shaders
{
    return UnityObjectToClipPos(pos.xyz);
}

#endif