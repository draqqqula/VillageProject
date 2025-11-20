Shader "Custom/InvisibleStencilWriter"
{
    Properties
    {
        _StencilRef("Stencil Reference", Int) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        // Отключаем освещение и цвет
        Lighting Off
        ColorMask 0       // Не записываем цвет в Render Target
        ZWrite Off        // Не пишем в Depth, можно включить при необходимости

        Stencil
        {
            Ref [_StencilRef]
            Comp Always       // Всегда записываем
            Pass Replace      // Записываем Ref в Stencil
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Ничего не рисуем (ColorMask 0 делает это)
                return fixed4(0,0,0,0);
            }
            ENDCG
        }
    }
}
