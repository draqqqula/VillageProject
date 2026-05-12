Shader "Hidden/TexelMarching/ReplaceExactColor4Neighbors"
{
    Properties
    {
        _TargetColor ("Target Color", Color) = (0, 0, 0, 1)
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            Name "ReplaceExactColor4Neighbors"

            ZWrite Off
            ZTest Always
            Cull Off

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D_X(_BlitTexture);

            float4 _BlitTexture_TexelSize;
            float4 _TargetColor;

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

            float4 SampleSource(float2 uv)
            {
                return SAMPLE_TEXTURE2D_X(
                    _BlitTexture,
                    sampler_PointClamp,
                    uv
                );
            }

            bool IsTargetColor(float4 color)
            {
                // Строгое сравнение RGB.
                // Alpha не сравниваю специально, потому что fullscreen/intermediate RT
                // часто имеют alpha = 1 или неопределённое значение.
                return
                    color.r == _TargetColor.r &&
                    color.g == _TargetColor.g &&
                    color.b == _TargetColor.b;
            }

            float4 Frag(Varyings input) : SV_Target
            {
                float2 texel = abs(_BlitTexture_TexelSize.xy);

                float4 center = SampleSource(input.uv);

                // Если центральный пиксель не выбранного цвета — не трогаем.
                if (!IsTargetColor(center))
                {
                    return center;
                }

                float4 left  = SampleSource(input.uv + float2(-texel.x, 0.0));
                float4 right = SampleSource(input.uv + float2( texel.x, 0.0));
                float4 up    = SampleSource(input.uv + float2(0.0,  texel.y));
                float4 down  = SampleSource(input.uv + float2(0.0, -texel.y));

                bool leftIsDifferent  = !IsTargetColor(left);
                bool rightIsDifferent = !IsTargetColor(right);
                bool upIsDifferent    = !IsTargetColor(up);
                bool downIsDifferent  = !IsTargetColor(down);

                // Заменяем только если ВСЕ 4 соседа имеют другой цвет.
                bool allFourNeighborsAreDifferent =
                    leftIsDifferent &&
                    rightIsDifferent &&
                    upIsDifferent &&
                    downIsDifferent;

                // if (!allFourNeighborsAreDifferent)
                // {
                //     return center;
                // }

                // Цвет выбирается в порядке:
                // left → right → up → down.
                //
                // Так как выше проверено, что все 4 отличаются,
                // фактически почти всегда вернётся left.
                if (leftIsDifferent)
                    return left;

                if (rightIsDifferent)
                    return right;

                if (upIsDifferent)
                    return up;

                if (downIsDifferent)
                    return down;

                return center;
            }

            ENDHLSL
        }
    }
}