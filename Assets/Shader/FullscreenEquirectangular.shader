Shader "Custom/FullscreenEquirectangular"
{
    Properties
    {
        _Tex ("Equirectangular Texture", 2D) = "white" {}
        _Exposure ("Exposure", Range(0, 8)) = 1.0
        _Rotation ("Rotation Y (degrees)", Range(0, 360)) = 0
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "FullscreenEquirectangular"
            ZWrite Off
            ZTest Always
            Cull Off

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_Tex);
            SAMPLER(sampler_Tex);

            float _Exposure;
            float _Rotation;

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

                // fullscreen triangle
                o.uv = float2((v.vertexID << 1) & 2, v.vertexID & 2);
                o.positionCS = float4(o.uv * 2.0 - 1.0, 0.0, 1.0);

                return o;
            }

            float3 GetViewDir(float2 uv)
            {
                // экран → NDC [-1,1]
                float2 ndc = uv * 2.0 - 1.0;

                float4 clip = float4(ndc, 1.0, 1.0);
                float4 view = mul(UNITY_MATRIX_I_P, clip);
                view /= view.w;

                float3 dirVS = normalize(view.xyz);

                // в world space
                float3 dirWS = mul((float3x3)UNITY_MATRIX_I_V, dirVS);

                return normalize(dirWS);
            }

            float3 RotateY(float3 dir, float degrees)
            {
                float rad = radians(degrees);
                float s = sin(rad);
                float c = cos(rad);

                float2 xz = float2(
                    dir.x * c - dir.z * s,
                    dir.x * s + dir.z * c
                );

                return float3(xz.x, dir.y, xz.y);
            }

            float2 DirToEquirectUV(float3 dir)
            {
                dir = normalize(dir);

                float phi = atan2(dir.x, dir.z);   // [-PI, PI]
                float theta = asin(dir.y);         // [-PI/2, PI/2]

                float2 uv;
                uv.x = (phi / (2.0 * PI)) + 0.5;
                uv.y = (theta / PI) + 0.5;

                return uv;
            }

            float4 Frag (Varyings i) : SV_Target
            {
                // 1. направление взгляда
                float3 dir = GetViewDir(i.uv);

                // 2. вращение
                dir = RotateY(dir, _Rotation);

                // 3. в equirect UV
                float2 uv = DirToEquirectUV(dir);

                // 4. sample
                float4 col = SAMPLE_TEXTURE2D(_Tex, sampler_Tex, uv);

                return col * _Exposure;
            }

            ENDHLSL
        }
    }
}