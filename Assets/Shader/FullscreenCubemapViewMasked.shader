Shader "Custom/FullscreenCubemapViewMasked"
{
    Properties
    {
        _Cube ("Cubemap", CUBE) = "" {}

        _BlitTexture ("Blit Texture", 2D) = "black" {}
        _TargetColor ("Target Color", Color) = (0, 0, 0, 1)

        _BackgroundColor ("Background Color", Color) = (0, 0, 0, 1)
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "FullscreenCubemapMaskedPass"

            ZWrite Off
            ZTest Always
            Cull Off

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURECUBE(_Cube);
            SAMPLER(sampler_Cube);

            TEXTURE2D_X(_BlitTexture);

            float4 _TargetColor;
            float4 _BackgroundColor;

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

                // fullscreen triangle
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
                    sampler_PointClamp,
                    blitUV
                );
            }

            bool IsTargetColor(float4 color)
            {
                // Без tolerance. Alpha не сравниваю.
                return
                    color.r == _TargetColor.r &&
                    color.g == _TargetColor.g &&
                    color.b == _TargetColor.b;
            }

            float3 GetViewDir(float2 uv01)
            {
                float2 ndc = uv01 * 2.0 - 1.0;

                float4 clip = float4(ndc, 1.0, 1.0);

                float4 view = mul(UNITY_MATRIX_I_P, clip);
                view /= view.w;

                return normalize(mul((float3x3)UNITY_MATRIX_I_V, view.xyz));
            }

            float4 AlphaBlendOver(float4 foreground, float4 background)
            {
                float alpha = saturate(foreground.a);

                float3 rgb = lerp(background.rgb, foreground.rgb, alpha);

                // Обычно итоговый fullscreen-пиксель считаем непрозрачным.
                return float4(rgb, 1.0);
            }

            float4 Frag(Varyings i) : SV_Target
            {
                float4 blitColor = SampleBlitTexture(i.uv);

                float3 dirWS = GetViewDir(i.uv);

                float4 cubemapColor = SAMPLE_TEXTURECUBE_LOD(
                    _Cube,
                    sampler_Cube,
                    dirWS,
                    0
                );

                // TargetColor = "покажи cubemap"
                [branch]
                if (IsTargetColor(blitColor))
                {
                    return cubemapColor;
                }

                // Если blitColor полупрозрачный, накладываем его на cubemap.
                [branch]
                if (blitColor.a < 1.0)
                {
                    return float4(0.0, 1.0, 0.0, 1.0);
                    return AlphaBlendOver(blitColor, cubemapColor);
                }

                // Иначе просто blitColor.
                return blitColor;
            }

            ENDHLSL
        }
    }
}