Shader "Custom/FullscreenCubemapViewReplaceByDistance"
{
    Properties
    {
        _Cube ("Cubemap", CUBE) = "" {}
        _DepthCube ("Depth Cubemap", CUBE) = "" {}

        _BlitTexture ("Blit Texture", 2D) = "black" {}

        // DepthCube convention:
        // 0 = near / min distance
        // 1 = max distance / sky
        _MaxDistance ("Max Distance", Float) = 100.0

        // Если decoded distance >= threshold, возвращаем _BlitTexture.
        _DistanceThreshold ("Distance Threshold", Float) = 100.0
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "FullscreenCubemapReplaceByDistancePass"

            ZWrite Off
            ZTest Always
            Cull Off

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURECUBE(_Cube);
            SAMPLER(sampler_Cube);

            TEXTURECUBE(_DepthCube);
            SAMPLER(sampler_DepthCube);

            TEXTURE2D_X(_BlitTexture);

            float _MaxDistance;
            float _DistanceThreshold;

            struct Attributes
            {
                uint vertexID : SV_VertexID;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert(Attributes v)
            {
                Varyings o;

                o.uv = float2((v.vertexID << 1) & 2, v.vertexID & 2);
                o.positionCS = float4(o.uv * 2.0 - 1.0, 0.0, 1.0);

                return o;
            }

            float4 SampleBlitTexture(float2 uv)
            {
                float2 blitUV = uv;

                #if UNITY_UV_STARTS_AT_TOP
                    blitUV.y = 1.0 - blitUV.y;
                #endif

                return SAMPLE_TEXTURE2D_X(
                    _BlitTexture,
                    sampler_LinearClamp,
                    blitUV
                );
            }

            float3 GetViewDir(float2 uv01)
            {
                float2 ndc = uv01 * 2.0 - 1.0;

                float4 clip = float4(ndc, 1.0, 1.0);

                float4 view = mul(UNITY_MATRIX_I_P, clip);
                view /= view.w;

                return normalize(mul((float3x3)UNITY_MATRIX_I_V, view.xyz));
            }

            float DecodeDistance(float depthSample)
            {
                return saturate(depthSample) * max(_MaxDistance, 0.0001);
            }

            float4 Frag(Varyings i) : SV_Target
            {
                float3 dirWS = GetViewDir(i.uv);

                float depthSample = SAMPLE_TEXTURECUBE_LOD(
                    _DepthCube,
                    sampler_DepthCube,
                    dirWS,
                    0
                ).r;

                float distance = DecodeDistance(depthSample);

                [branch]
                if (distance >= _DistanceThreshold)
                {
                    return SampleBlitTexture(i.uv);
                }

                return SAMPLE_TEXTURECUBE_LOD(
                    _Cube,
                    sampler_Cube,
                    dirWS,
                    0
                );
            }

            ENDHLSL
        }
    }
}