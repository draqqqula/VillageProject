Shader "Custom/CubemapRadialDepth"
{
    Properties
    {
        _DepthMax ("Depth Max", Float) = 100.0
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

            float3 _CubemapOrigin;
            float _DepthMax;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float dist = distance(i.worldPos, _CubemapOrigin);

                float normalized = saturate(dist / _DepthMax);

                return float4(normalized, 0, 0, 1);
            }

            ENDHLSL
        }
    }
}