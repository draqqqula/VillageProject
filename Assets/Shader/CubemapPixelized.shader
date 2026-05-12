Shader "Custom/URP/CubemapPixelated_Working"
{
    Properties
    {
        _Cube ("Cubemap", CUBE) = "" {}
        _Exposure ("Exposure", Float) = 1.0
        _PixelSize ("Pixel Size", Float) = 32.0
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

        // -----------------------------
        // Основной Forward Pass
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
            float _PixelSize;

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 dirWS : TEXCOORD0;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                float3 centerWS = TransformObjectToWorld(float3(0,0,0));
                OUT.dirWS = normalize(worldPos - centerWS);
                OUT.positionHCS = TransformWorldToHClip(worldPos);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float3 dir = IN.dirWS;

                // ----- Пикселизация через сферические координаты -----
                float theta = acos(dir.y);
                float phi = atan2(dir.z, dir.x);

                theta = round(theta * _PixelSize) / _PixelSize;
                phi   = round((phi + 3.14159265) * _PixelSize / (2.0*3.14159265)) / _PixelSize * 2.0*3.14159265 - 3.14159265;

                float sinTheta = sin(theta);
                dir = float3(cos(phi)*sinTheta, cos(theta), sin(phi)*sinTheta);

                // 🔹 Семплирование Cubemap (Cubemap должна быть в Point filter)
                float4 col = SAMPLE_TEXTURECUBE(_Cube, sampler_Cube, dir);
                col.rgb *= _Exposure;

                return col;
            }

            ENDHLSL
        }

        // -----------------------------
        // Depth pass (обязателен для URP + RenderGraph)
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

            Varyings vert(Attributes IN)
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