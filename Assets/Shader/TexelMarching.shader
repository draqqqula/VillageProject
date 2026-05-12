Shader "Custom/TexelMarchingFullscreen"
{
    Properties
    {
        _ColorCube ("Color Cubemap", CUBE) = "" {}
        _DepthCube ("Depth Cubemap", CUBE) = "" {}
        _FallbackCube ("Fallback Cubemap", CUBE) = "" {}

        _CubemapOffset ("Cubemap Offset (World)", Vector) = (0,0,0,0)
        _DepthMax ("Depth Max", Float) = 100
        _MaxSteps ("Max Steps", Int) = 128
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "TexelMarchingPass"
            ZWrite Off
            ZTest Always
            Cull Off

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURECUBE(_ColorCube);
            SAMPLER(sampler_ColorCube);

            TEXTURECUBE(_DepthCube);
            SAMPLER(sampler_DepthCube);

            TEXTURECUBE(_FallbackCube);
            SAMPLER(sampler_FallbackCube);

            float3 _CubemapOffset;
            float _DepthMax;
            int _MaxSteps;

            #define MAX_STEPS 1024

            struct Attributes
            {
                uint vertexID : SV_VertexID;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert (Attributes v)
            {
                Varyings o;

                o.uv = float2((v.vertexID << 1) & 2, v.vertexID & 2);
                o.positionCS = float4(o.uv * 2.0 - 1.0, 0.0, 1.0);

                return o;
            }

            float3 GetViewDir(float2 uv)
            {
                float2 ndc = uv * 2.0 - 1.0;

                float4 clip = float4(ndc, 1.0, 1.0);
                float4 view = mul(UNITY_MATRIX_I_P, clip);
                view /= view.w;

                return normalize(mul((float3x3)UNITY_MATRIX_I_V, view.xyz));
            }

            float4 RaymarchCubemap(float3 dir)
            {
                float maxTravel = _DepthMax;

                int steps = max(_MaxSteps, 1);
                float stepBase = pow(maxTravel, 1.0 / steps);
                float progress = stepBase;

                [loop]
                for (int i = 0; i < MAX_STEPS; i++)
                {
                    if (i >= steps)
                        break;

                    float t = progress;


                    float3 posFromCube = _CubemapOffset + dir * t;

                    float pointDist = length(posFromCube);
                    float3 cubeDir = posFromCube / max(pointDist, 1e-6);

                    float3 sampleDir = cubeDir;

                    float surfaceDist =
                        SAMPLE_TEXTURECUBE_LOD(_DepthCube, sampler_DepthCube, sampleDir, 0).r * _DepthMax;

                    float stepSize = progress - progress / stepBase;

                    if (pointDist >= surfaceDist)
                    {
                        if (pointDist > surfaceDist + stepSize)
                        {
                            return SAMPLE_TEXTURECUBE_LOD(_FallbackCube, sampler_FallbackCube, dir, 0);
                        }

                        return SAMPLE_TEXTURECUBE_LOD(_ColorCube, sampler_ColorCube, sampleDir, 0);
                    }

                    progress *= stepBase;
                }

                return float4(0, 0, 1, 1);
            }

            float4 Frag (Varyings i) : SV_Target
            {
                float3 dir = GetViewDir(i.uv);

                return RaymarchCubemap(dir);
            }

            ENDHLSL
        }
    }
}