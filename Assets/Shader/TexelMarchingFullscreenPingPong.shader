Shader "Custom/TexelMarchingFullscreenPingPong"
{
    Properties
    {
        _ColorCubePing ("Color Cubemap Ping", CUBE) = "" {}
        _DepthCubePing ("Depth Cubemap Ping", CUBE) = "" {}
        _FallbackCubePing ("Fallback Cubemap Ping", CUBE) = "" {}

        _ColorCubePong ("Color Cubemap Pong", CUBE) = "" {}
        _DepthCubePong ("Depth Cubemap Pong", CUBE) = "" {}
        _FallbackCubePong ("Fallback Cubemap Pong", CUBE) = "" {}

        _CubemapOffsetPing ("Cubemap Offset Ping (Viewer From Cubemap Origin)", Vector) = (0,0,0,0)
        _CubemapOffsetPong ("Cubemap Offset Pong (Viewer From Cubemap Origin)", Vector) = (0,0,0,0)

        _PostProcessTargetColor ("Post Process Target Color", Color) = (0,0,0,1)
        _UseRaymarchPostProcess ("Use Raymarch Post Process", Float) = 1

        _Blend ("Ping/Pong Blend", Range(0,1)) = 0

        _DepthNear ("Depth Near", Float) = 0.1
        _DepthFar ("Depth Far", Float) = 100
        _MaxSteps ("Max Steps", Int) = 128

        _SkyboxMask ("Skybox Mask", 2D) = "black" {}
        _SkyboxMaskThreshold ("Skybox Mask Threshold", Range(0,1)) = 0.6

        _FallbackMaskThreshold ("Fallback Mask Threshold", Range(0,1)) = 0.001

        _BlitTexture ("Blit Texture", 2D) = "black" {}
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "TexelMarchingPingPongPass"
            ZWrite Off
            ZTest Always
            Cull Off

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURECUBE(_ColorCubePing);
            SAMPLER(sampler_ColorCubePing);

            TEXTURECUBE(_DepthCubePing);
            SAMPLER(sampler_DepthCubePing);

            TEXTURECUBE(_FallbackCubePing);
            SAMPLER(sampler_FallbackCubePing);

            TEXTURECUBE(_ColorCubePong);
            SAMPLER(sampler_ColorCubePong);

            TEXTURECUBE(_DepthCubePong);
            SAMPLER(sampler_DepthCubePong);

            TEXTURECUBE(_FallbackCubePong);
            SAMPLER(sampler_FallbackCubePong);

            TEXTURE2D(_SkyboxMask);
            SAMPLER(sampler_SkyboxMask);

            TEXTURE2D_X(_BlitTexture);
            float4 _BlitTexture_TexelSize;

            float3 _CubemapOffsetPing;
            float3 _CubemapOffsetPong;

            float _Blend;
            float _DepthNear;
            float _DepthFar;
            int _MaxSteps;

            float _SkyboxMaskThreshold;
            float _FallbackMaskThreshold;

            float4 _PostProcessTargetColor;
            float _UseRaymarchPostProcess;

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

            Varyings Vert(Attributes v)
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

            float SmoothBlend(float t)
            {
                t = saturate(t);
                return t * t * (3.0 - 2.0 * t);
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

            float4 SampleFallbackOrBlit(
                TEXTURECUBE(fallbackCube), SAMPLER(samplerFallbackCube),
                float3 dir,
                float maskValue,
                float2 screenUV
            )
            {
                // convention:
                // mask = 0.0 → object / fallback разрешён
                // mask > 0.0 → outline или sky / fallback запрещён
                //
                // Если fallback запрещён, возвращаем исходный fullscreen color.
                [branch]
                if (maskValue > _FallbackMaskThreshold)
                {
                    return SampleBlitTexture(screenUV);
                }

                float4 color = SAMPLE_TEXTURECUBE_LOD(
                    fallbackCube,
                    samplerFallbackCube,
                    dir,
                    0
                );

                [branch]
                if (color.r == 1.0 & color.g == 0.0 & color.b == 1.0 & color.a == 1.0)
                {
                    return SampleBlitTexture(screenUV);
                }

                return color;
            }

            float DecodeDepthDistance(float depthSample)
            {
                return lerp(_DepthNear, _DepthFar, saturate(depthSample));
            }

            bool IsDepthSkyOrFar(float depthSample)
            {
                // Depth shader пишет sky как 1.
                // Геометрия ровно на far тоже станет 1, но в этом случае fallback допустим.
                return depthSample >= 0.99999;
            }

            float4 RaymarchCubemap(
                TEXTURECUBE(colorCube), SAMPLER(samplerColorCube),
                TEXTURECUBE(depthCube), SAMPLER(samplerDepthCube),
                TEXTURECUBE(fallbackCube), SAMPLER(samplerFallbackCube),
                float3 cubemapOffset,
                float3 dir,
                float maskValue,
                float2 screenUV
            )
            {
                int steps = clamp(_MaxSteps, 1, MAX_STEPS);

                float depthNear = max(_DepthNear, 0.0);
                float depthFar = max(_DepthFar, depthNear + 0.0001);
                float maxDistance = max(depthFar - depthNear, 0.0001);

                float stepBase = pow(maxDistance + 1.0, 1.0 / steps);
                float previousProgress = 0.0;
                float progress = stepBase - 1.0;

                [loop]
                for (int i = 0; i < MAX_STEPS; i++)
                {
                    if (i >= steps)
                        break;

                    float t = depthNear + progress;

                    float3 posFromCubemapOrigin = cubemapOffset + dir * t;

                    float pointDist = length(posFromCubemapOrigin);

                    if (pointDist < 0.000001)
                    {
                        previousProgress = progress;
                        progress = (progress + 1.0) * stepBase - 1.0;
                        continue;
                    }

                    float3 cubeDir = posFromCubemapOrigin / pointDist;
                    float3 sampleDir = cubeDir;

                    float depthSample =
                        SAMPLE_TEXTURECUBE_LOD(depthCube, samplerDepthCube, sampleDir, 0).r;

                    float surfaceDist = DecodeDepthDistance(depthSample);

                    float stepSize = max(progress - previousProgress, 0.0001);

                    if (pointDist >= surfaceDist)
                    {
                        if (pointDist > surfaceDist + stepSize || IsDepthSkyOrFar(depthSample))
                        {
                            return SampleFallbackOrBlit(
                                fallbackCube,
                                samplerFallbackCube,
                                dir,
                                maskValue,
                                screenUV
                            );
                        }

                        return SAMPLE_TEXTURECUBE_LOD(
                            colorCube,
                            samplerColorCube,
                            sampleDir,
                            0
                        );
                    }

                    previousProgress = progress;
                    progress = (progress + 1.0) * stepBase - 1.0;
                }

                return SampleFallbackOrBlit(
                    fallbackCube,
                    samplerFallbackCube,
                    dir,
                    maskValue,
                    screenUV
                );
            }

            bool IsPostProcessTargetColor(float4 color)
            {
                // Строгое сравнение RGB.
                // Alpha не сравниваем, чтобы не зависеть от alpha в cubemap/blit.
                return
                    color.r == _PostProcessTargetColor.r &&
                    color.g == _PostProcessTargetColor.g &&
                    color.b == _PostProcessTargetColor.b;
            }

            float4 BlendRaymarchColors(float4 colorPing, float4 colorPong, float blend)
            {
                bool pingIsTarget = IsPostProcessTargetColor(colorPing);
                bool pongIsTarget = IsPostProcessTargetColor(colorPong);

                // Ping невалидный / target → берём Pong без смешивания.
                [branch]
                if (pingIsTarget && !pongIsTarget)
                {
                    return colorPing;
                }

                // Pong невалидный / target → берём Ping без смешивания.
                [branch]
                if (pongIsTarget && !pingIsTarget)
                {
                    return colorPong;
                }

                // Оба target — смешивать нечего.
                // Оставляем target color, но alpha всё равно равен blend.
                [branch]
                if (pingIsTarget && pongIsTarget)
                {
                    return colorPing;
                }

                // Обычный случай.
                return lerp(colorPing, colorPong, blend);
            }

            float SampleMaskAtScreenUV(float2 screenUV)
            {
                float2 maskUV = screenUV;

                // Такой же flip, как в Frag.
                maskUV.y = 1.0 - maskUV.y;

                return SAMPLE_TEXTURE2D(
                    _SkyboxMask,
                    sampler_SkyboxMask,
                    maskUV
                ).r;
            }

            float4 PaymarchCubemapPostProcess(
                TEXTURECUBE(colorCube), SAMPLER(samplerColorCube),
                TEXTURECUBE(depthCube), SAMPLER(samplerDepthCube),
                TEXTURECUBE(fallbackCube), SAMPLER(samplerFallbackCube),
                float3 cubemapOffset,
                float3 dir,
                float maskValue,
                float2 screenUV
            )
            {
                float4 center = RaymarchCubemap(
                    colorCube, samplerColorCube,
                    depthCube, samplerDepthCube,
                    fallbackCube, samplerFallbackCube,
                    cubemapOffset,
                    dir,
                    maskValue,
                    screenUV
                );

                [branch]
                if (maskValue < 0.4)
                {
                    return center;
                }

                [branch]
                if (_UseRaymarchPostProcess < 0.5)
                {
                    return center;
                }

                // Если центральный пиксель не выбранного цвета — не трогаем.
                [branch]
                if (!IsPostProcessTargetColor(center))
                {
                    return center;
                }

                float2 texel = abs(_BlitTexture_TexelSize.xy);

                float2 uvLeft  = screenUV + float2(-texel.x, 0.0);
                float2 uvRight = screenUV + float2( texel.x, 0.0);
                float2 uvUp    = screenUV + float2(0.0,  texel.y);
                float2 uvDown  = screenUV + float2(0.0, -texel.y);

                float3 dirLeft  = GetViewDir(uvLeft);
                float3 dirRight = GetViewDir(uvRight);
                float3 dirUp    = GetViewDir(uvUp);
                float3 dirDown  = GetViewDir(uvDown);

                float maskLeft  = SampleMaskAtScreenUV(uvLeft);
                float maskRight = SampleMaskAtScreenUV(uvRight);
                float maskUp    = SampleMaskAtScreenUV(uvUp);
                float maskDown  = SampleMaskAtScreenUV(uvDown);

                float4 left = RaymarchCubemap(
                    colorCube, samplerColorCube,
                    depthCube, samplerDepthCube,
                    fallbackCube, samplerFallbackCube,
                    cubemapOffset,
                    dirLeft,
                    maskLeft,
                    uvLeft
                );

                if (!IsPostProcessTargetColor(left))
                    return left;

                float4 right = RaymarchCubemap(
                    colorCube, samplerColorCube,
                    depthCube, samplerDepthCube,
                    fallbackCube, samplerFallbackCube,
                    cubemapOffset,
                    dirRight,
                    maskRight,
                    uvRight
                );

                if (!IsPostProcessTargetColor(right))
                    return right;

                float4 up = RaymarchCubemap(
                    colorCube, samplerColorCube,
                    depthCube, samplerDepthCube,
                    fallbackCube, samplerFallbackCube,
                    cubemapOffset,
                    dirUp,
                    maskUp,
                    uvUp
                );

                if (!IsPostProcessTargetColor(up))
                    return up;

                float4 down = RaymarchCubemap(
                    colorCube, samplerColorCube,
                    depthCube, samplerDepthCube,
                    fallbackCube, samplerFallbackCube,
                    cubemapOffset,
                    dirDown,
                    maskDown,
                    uvDown
                );

                if (!IsPostProcessTargetColor(down))
                    return down;

                return center;
            }

            float4 RaymarchCubemapPingPong(float3 dir, float maskValue, float2 screenUV)
            {
                float rawBlend = saturate(_Blend);

                // 0 = полностью Ping. Pong вообще не считаем.
                [branch]
                if (rawBlend <= 0.0001)
                {
                    return PaymarchCubemapPostProcess(
                        _ColorCubePing, sampler_ColorCubePing,
                        _DepthCubePing, sampler_DepthCubePing,
                        _FallbackCubePing, sampler_FallbackCubePing,
                        _CubemapOffsetPing,
                        dir,
                        maskValue,
                        screenUV
                    );
                }

                // 1 = полностью Pong. Ping вообще не считаем.
                [branch]
                if (rawBlend >= 0.9999)
                {
                    return PaymarchCubemapPostProcess(
                        _ColorCubePong, sampler_ColorCubePong,
                        _DepthCubePong, sampler_DepthCubePong,
                        _FallbackCubePong, sampler_FallbackCubePong,
                        _CubemapOffsetPong,
                        dir,
                        maskValue,
                        screenUV
                    );
                }

                // Только во время реального перехода считаем оба.
                float4 colorPing = PaymarchCubemapPostProcess(
                    _ColorCubePing, sampler_ColorCubePing,
                    _DepthCubePing, sampler_DepthCubePing,
                    _FallbackCubePing, sampler_FallbackCubePing,
                    _CubemapOffsetPing,
                    dir,
                    maskValue,
                    screenUV
                );

                float4 colorPong = PaymarchCubemapPostProcess(
                    _ColorCubePong, sampler_ColorCubePong,
                    _DepthCubePong, sampler_DepthCubePong,
                    _FallbackCubePong, sampler_FallbackCubePong,
                    _CubemapOffsetPong,
                    dir,
                    maskValue,
                    screenUV
                );

                float blend = SmoothBlend(rawBlend);

                return BlendRaymarchColors(colorPing, colorPong, blend);
            }

            float4 Frag(Varyings i) : SV_Target
            {
                float3 dir = GetViewDir(i.uv);

                float2 maskUV = i.uv;

                // Оставил flip как в твоём текущем shader.
                maskUV.y = 1.0 - maskUV.y;

                float skyMask = SAMPLE_TEXTURE2D(
                    _SkyboxMask,
                    sampler_SkyboxMask,
                    maskUV
                ).r;

                // Чистый sky: вообще не делаем raymarch.
                // Вместо чёрного возвращаем исходный fullscreen color.
                [branch]
                if (skyMask > _SkyboxMaskThreshold)
                {
                    return SampleBlitTexture(i.uv);
                }

                // object/edge зона:
                // - mask == 0.0: raymarch + fallback разрешён
                // - mask == 0.5: raymarch разрешён, fallback запрещён,
                //                при miss/overshoot вернётся _BlitTexture
                return RaymarchCubemapPingPong(dir, skyMask, i.uv);
            }

            ENDHLSL
        }
    }
}