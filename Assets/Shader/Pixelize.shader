Shader "Hidden/Pixelize"
{
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "Pixelation"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float2 _BlockCount;
            float2 _BlockSize;
            float2 _HalfBlockSize;

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings vert(uint vertexID : SV_VertexID)
            {
                Varyings OUT;
                OUT.positionHCS = GetFullScreenTriangleVertexPosition(vertexID);
                OUT.uv = GetFullScreenTriangleTexCoord(vertexID);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_TARGET
            {
                float2 blockPos = floor(IN.uv * _BlockCount);
                float2 blockCenter = blockPos * _BlockSize + _HalfBlockSize;

                return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, blockCenter);
            }

            ENDHLSL
        }
    }
}