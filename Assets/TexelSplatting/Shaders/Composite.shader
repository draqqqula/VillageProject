Shader "Hidden/TexelSplatting/Composite"
{
    Properties
    {
        _MainTex ("Splat Texture", 2D) = "black" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            ZWrite Off
            ZTest Always
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURECUBE(_CameraCubemap);
            SAMPLER(sampler_CameraCubemap);

            float4x4 _TS_InvViewProjMatrix;
            float    _PosterizeLevels;
            float3   _CameraWorldPos;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                float4 splatColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);

                if (splatColor.a > 0.0)
                {
                    return float4(splatColor.rgb, 1.0);
                }

                // Disocclusion fallback: reconstruct view direction and sample camera cubemap
                float2 ndc = input.uv * 2.0 - 1.0;
                float4 clipPos = float4(ndc, 0.5, 1.0);
                float4 worldPos = mul(_TS_InvViewProjMatrix, clipPos);
                worldPos /= worldPos.w;
                float3 viewDir = normalize(worldPos.xyz - _CameraWorldPos);

                float4 cubeColor = SAMPLE_TEXTURECUBE_LOD(_CameraCubemap, sampler_CameraCubemap, viewDir, 0);

                // Posterize the fallback
                float levels = _PosterizeLevels;
                cubeColor.rgb = floor(cubeColor.rgb * levels + 0.5) / levels;

                return float4(cubeColor.rgb, 1.0);
            }

            ENDHLSL
        }
    }
}
