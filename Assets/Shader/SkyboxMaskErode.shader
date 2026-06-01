Shader "Hidden/TexelMarching/SkyboxMaskErodeFast"
{
    Properties
    {
        _OuterMarginScreenPixels ("Outer Margin Screen Pixels", Float) = 4
        _InnerMarginScreenPixels ("Inner Margin Screen Pixels", Float) = 2
        _OutlineValue ("Outline Value", Range(0,1)) = 0.5
        _UseDiagonals ("Use Diagonals", Float) = 1

        // Нужно выставлять из C#:
        // x = full screen width
        // y = full screen height
        _ScreenResolution ("Screen Resolution", Vector) = (1920,1080,0,0)
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            Name "SkyboxMaskErodeFast"

            ZWrite Off
            ZTest Always
            Cull Off

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D_X(_BlitTexture);

            float4 _BlitTexture_TexelSize;

            float _OuterMarginScreenPixels;
            float _InnerMarginScreenPixels;
            float _OutlineValue;
            float _UseDiagonals;
            float4 _ScreenResolution;

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

            float SampleMaskPoint(float2 uv)
            {
                return SAMPLE_TEXTURE2D_X(
                    _BlitTexture,
                    sampler_PointClamp,
                    uv
                ).r;
            }

            float SampleMaskLinear(float2 uv)
            {
                return SAMPLE_TEXTURE2D_X(
                    _BlitTexture,
                    sampler_LinearClamp,
                    uv
                ).r;
            }

            float2 ScreenPixelsToUvOffset(float screenPixels)
            {
                float2 maskSize = abs(_BlitTexture_TexelSize.zw);
                float2 screenSize = max(_ScreenResolution.xy, float2(1.0, 1.0));

                // screenPixels задан в пикселях финального экрана.
                // Переводим в пиксели текущей mask texture.
                float2 marginMaskPixels = screenPixels * (maskSize / screenSize);

                return abs(_BlitTexture_TexelSize.xy) * marginMaskPixels;
            }

            float MinAround(float2 uv, float2 o)
            {
                float result = SampleMaskLinear(uv);

                result = min(result, SampleMaskLinear(uv + float2( o.x, 0.0)));
                result = min(result, SampleMaskLinear(uv + float2(-o.x, 0.0)));
                result = min(result, SampleMaskLinear(uv + float2(0.0,  o.y)));
                result = min(result, SampleMaskLinear(uv + float2(0.0, -o.y)));

                if (_UseDiagonals > 0.5)
                {
                    result = min(result, SampleMaskLinear(uv + float2( o.x,  o.y)));
                    result = min(result, SampleMaskLinear(uv + float2(-o.x,  o.y)));
                    result = min(result, SampleMaskLinear(uv + float2( o.x, -o.y)));
                    result = min(result, SampleMaskLinear(uv + float2(-o.x, -o.y)));
                }

                return result;
            }

            float MaxAround(float2 uv, float2 o)
            {
                float result = SampleMaskLinear(uv);

                result = max(result, SampleMaskLinear(uv + float2( o.x, 0.0)));
                result = max(result, SampleMaskLinear(uv + float2(-o.x, 0.0)));
                result = max(result, SampleMaskLinear(uv + float2(0.0,  o.y)));
                result = max(result, SampleMaskLinear(uv + float2(0.0, -o.y)));

                if (_UseDiagonals > 0.5)
                {
                    result = max(result, SampleMaskLinear(uv + float2( o.x,  o.y)));
                    result = max(result, SampleMaskLinear(uv + float2(-o.x,  o.y)));
                    result = max(result, SampleMaskLinear(uv + float2( o.x, -o.y)));
                    result = max(result, SampleMaskLinear(uv + float2(-o.x, -o.y)));
                }

                return result;
            }

            float4 Frag(Varyings input) : SV_Target
            {
                // Raw convention:
                // sky    = 1
                // object = 0
                float center = SampleMaskPoint(input.uv);

                bool isSky = center >= 0.5;
                bool isObject = !isSky;

                float outlineValue = saturate(_OutlineValue);

                // Наружная зона:
                // если текущий пиксель sky, но рядом есть object,
                // превращаем sky в серый outline.
                if (isSky && _OuterMarginScreenPixels > 0.001)
                {
                    float2 outerOffset = ScreenPixelsToUvOffset(_OuterMarginScreenPixels);
                    float minAround = MinAround(input.uv, outerOffset);

                    bool hasObjectNearby = minAround < 0.5;

                    if (hasObjectNearby)
                    {
                        return float4(outlineValue, outlineValue, outlineValue, 1.0);
                    }
                }

                // Внутренняя зона:
                // если текущий пиксель object, но рядом есть sky,
                // превращаем внутренний край object в серый outline.
                if (isObject && _InnerMarginScreenPixels > 0.001)
                {
                    float2 innerOffset = ScreenPixelsToUvOffset(_InnerMarginScreenPixels);
                    float maxAround = MaxAround(input.uv, innerOffset);

                    bool hasSkyNearby = maxAround >= 0.5;

                    if (hasSkyNearby)
                    {
                        return float4(outlineValue, outlineValue, outlineValue, 1.0);
                    }
                }

                return float4(center, center, center, 1.0);
            }

            ENDHLSL
        }
    }
}