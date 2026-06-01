Shader "Custom/URP/CubemapInsideSphere_Final"
{
    Properties
    {
        _Cube ("Cubemap", CUBE) = "" {}
        _Exposure ("Exposure", Float) = 1.0
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "Queue"="Geometry"
            "RenderPipeline"="UniversalPipeline"
            "UniversalMaterialType"="Unlit"
        }

        // 🔹 Основной pass
        Pass
        {
            Name "Forward"
            Tags { "LightMode"="UniversalForward" }

            Cull Front
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURECUBE(_Cube);
            SAMPLER(sampler_Cube);

            float _Exposure;

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 dirWS : TEXCOORD0;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;

                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);

                // 🔥 КЛЮЧ: направление от центра сферы
                float3 centerWS = TransformObjectToWorld(float3(0,0,0));
                OUT.dirWS = normalize(worldPos - centerWS);

                OUT.positionHCS = TransformWorldToHClip(worldPos);

                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float4 col = SAMPLE_TEXTURECUBE(_Cube, sampler_Cube, IN.dirWS);
                col.rgb *= _Exposure;
                return col;
            }

            ENDHLSL
        }

        // 🔹 Depth pass (обязателен для URP + RenderGraph)
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode"="DepthOnly" }

            ZWrite On
            ColorMask 0

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment fragDepth

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            half4 fragDepth(Varyings IN) : SV_Target
            {
                return 0;
            }

            ENDHLSL
        }
    }
}