Shader "Hidden/DepthCopyToRed"
{
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }
        ZTest Always
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct VIn
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct VOut
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);

            VOut vert(VIn v)
            {
                VOut o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            half4 frag(VOut i) : SV_Target
            {
                float d = SAMPLE_DEPTH_TEXTURE(
                    _CameraDepthTexture,
                    sampler_CameraDepthTexture,
                    i.uv
                );

                // если нужен линейный depth:
                // d = Linear01Depth(d, _ZBufferParams);

                return half4(d, 0, 0, 1);
            }
            ENDHLSL
        }
    }
}