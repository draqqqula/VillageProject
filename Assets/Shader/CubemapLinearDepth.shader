Shader "Custom/CubemapLinearDepth"
{
    Properties
    {
        _DepthMax ("Depth Max", Float) = 10.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float _DepthMax;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float depth : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;

                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                float4 viewPos = mul(UNITY_MATRIX_V, worldPos);

                o.pos = UnityObjectToClipPos(v.vertex);

                // Линейная глубина (расстояние вдоль взгляда камеры)
                o.depth = -viewPos.z;

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float normalized = saturate(i.depth / _DepthMax);
                return float4(normalized, 0, 0, 1);
            }

            ENDHLSL
        }
    }
}