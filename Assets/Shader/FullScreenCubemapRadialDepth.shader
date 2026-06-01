Shader "Custom/FullscreenCubemapRadialDepth"
{
    Properties
    {
        _DepthNear ("Depth Near", Float) = 0.1
        _DepthFar ("Depth Far", Float) = 100.0
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
        }

        Pass
        {
            Name "Fullscreen Cubemap Radial Depth"

            ZWrite Off
            ZTest Always
            Cull Off

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            float3 _CubemapOrigin;
            float _DepthNear;
            float _DepthFar;

            struct Attributes
            {
                uint vertexID : SV_VertexID;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;

                output.uv = float2(
                    (input.vertexID << 1) & 2,
                    input.vertexID & 2
                );

                output.positionCS = float4(
                    output.uv * 2.0 - 1.0,
                    0.0,
                    1.0
                );

                #if UNITY_UV_STARTS_AT_TOP
                output.uv.y = 1.0 - output.uv.y;
                #endif

                return output;
            }

            float4 Frag(Varyings input) : SV_Target
            {
                float2 uv = input.uv;

                float rawDepth = SampleSceneDepth(uv);

                #if UNITY_REVERSED_Z
                    bool isSky = rawDepth <= 0.00001;
                    float deviceDepth = rawDepth;
                #else
                    bool isSky = rawDepth >= 0.99999;
                    float deviceDepth = lerp(UNITY_NEAR_CLIP_VALUE, 1.0, rawDepth);
                #endif

                // Sky / no geometry хранится как Far.
                if (isSky)
                {
                    return float4(1.0, 0.0, 0.0, 1.0);
                }

                float3 worldPos = ComputeWorldSpacePosition(
                    uv,
                    deviceDepth,
                    UNITY_MATRIX_I_VP
                );

                float radialDepth = distance(worldPos, _CubemapOrigin);

                float depthRange = max(_DepthFar - _DepthNear, 0.0001);
                float normalized = saturate((radialDepth - _DepthNear) / depthRange);

                return float4(normalized, 0.0, 0.0, 1.0);
            }

            ENDHLSL
        }
    }
}