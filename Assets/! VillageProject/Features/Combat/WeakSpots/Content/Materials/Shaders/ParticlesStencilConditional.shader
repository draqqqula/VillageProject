Shader "Custom/ParticlesThroughStencil"
{
    Properties
    {
        _MainTex("Particle Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _StencilRef("Stencil Reference", Int) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }

        // Отключаем запись в Depth, стандартное альфа-смешивание
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        // Pass 1: рендерим частицы сквозь сферу
        Pass
        {
            Name "ThroughStencil"
            ZTest Always

            Stencil
            {
                Ref [_StencilRef]
                Comp Equal      // Только там, где Stencil сферы равен Ref
                Pass Keep
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv) * _Color;
            }
            ENDCG
        }

        // Pass 2: обычный рендер частиц в сцене
        Pass
        {
            Name "Normal"
            ZTest LEqual

            Stencil
            {
                Ref [_StencilRef]
                Comp NotEqual   // Рендерим только там, где Stencil != Ref
                Pass Keep
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv) * _Color;
            }
            ENDCG
        }
    }
}