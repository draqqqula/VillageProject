Shader "Custom/FullscreenCubemapRadial"
{
    Properties
    {
        _Cube ("Cubemap", CUBE) = "" {}
        _Fov ("Radial FOV", Range(0.1, 3.14159)) = 1.5708 // ~90°
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "FullscreenCubemapRadial"
            ZWrite Off
            ZTest Always
            Cull Off

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURECUBE(_Cube);
            SAMPLER(sampler_Cube);

            float _Fov;

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

            float3 GetRadialViewDir(float2 uv)
            {
                // [-1,1]
                float2 p = uv * 2.0 - 1.0;

                float r = length(p);

                // защита от деления на 0
                if (r < 1e-5)
                    return float3(0, 0, 1);

                // нормализуем направление в плоскости
                float2 dir = p / r;

                // радиальный угол
                float theta = r * _Fov;

                // fisheye-подобная реконструкция
                float sinT = sin(theta);
                float cosT = cos(theta);

                float3 viewDirVS = float3(dir * sinT, cosT);

                return viewDirVS;
            }

            float4 Frag (Varyings i) : SV_Target
            {
                // получаем radial направление во view space
                float3 dirVS = GetRadialViewDir(i.uv);

                // переводим в world space
                float3 dirWS = normalize(mul((float3x3)UNITY_MATRIX_I_V, dirVS));

                // семплим cubemap
                float4 color = SAMPLE_TEXTURECUBE(_Cube, sampler_Cube, dirWS);

                return color;
            }

            ENDHLSL
        }
    }
}