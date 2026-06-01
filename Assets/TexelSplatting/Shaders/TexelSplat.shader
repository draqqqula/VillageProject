Shader "Hidden/TexelSplatting/TexelSplat"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }

        Pass
        {
            ZWrite On
            ZTest LEqual
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct SplatData
            {
                float3 position;
                float  size;
                float4 color;
            };

            StructuredBuffer<SplatData> _SplatBuffer;

            struct Attributes
            {
                float4 positionOS : POSITION;
                uint instanceID : SV_InstanceID;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 color : TEXCOORD0;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;

                SplatData splat = _SplatBuffer[input.instanceID];

                float3 right = UNITY_MATRIX_V[0].xyz;
                float3 up    = UNITY_MATRIX_V[1].xyz;

                float3 worldPos = splat.position
                    + right * input.positionOS.x * splat.size
                    + up    * input.positionOS.y * splat.size;

                output.positionCS = TransformWorldToHClip(worldPos);
                output.color = splat.color;

                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                return float4(input.color.rgb, 1.0);
            }

            ENDHLSL
        }
    }
}
