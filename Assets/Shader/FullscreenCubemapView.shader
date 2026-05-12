    Shader "Custom/FullscreenCubemapView"
    {
    Properties
    {
        _Cube ("Cubemap", CUBE) = "" {}
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            Name "FullscreenCubemapPass"
            ZWrite Off
            ZTest Always
            Cull Off

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURECUBE(_Cube);
            SAMPLER(sampler_Cube);

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

            float4 Frag (Varyings i) : SV_Target
            {
                // экранные UV -> NDC [-1,1]
                float2 uv = i.uv * 2.0 - 1.0;

                // clip space
                float4 clip = float4(uv, 1.0, 1.0);

                // view space
                float4 view = mul(UNITY_MATRIX_I_P, clip);
                view /= view.w;

                // world direction
                float3 dirWS = normalize(mul((float3x3)UNITY_MATRIX_I_V, view.xyz));

                // sample cubemap
                float4 color = SAMPLE_TEXTURECUBE(_Cube, sampler_Cube, dirWS);

                return color;
            }

            ENDHLSL
        }
    }
}