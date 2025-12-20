Shader "Unlit/SpriteSilhouetteAdditive"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1) // ここは白推奨（強さだけ使う）
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "CanUseSpriteAtlas"="True" }
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha One

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                fixed4 color  : COLOR;      // ★追加：SpriteRenderer.color がここに入る
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv     : TEXCOORD0;
                fixed4 color  : COLOR;      // ★追加
            };

            sampler2D _MainTex;
            fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;          // ★追加
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed a = tex2D(_MainTex, i.uv).a;

                // 色は SpriteRenderer.color（i.color）で決まる
                fixed3 rgb = i.color.rgb * _Color.rgb;

                // alpha は (renderer alpha) × (material alpha) × (sprite alpha)
                fixed alpha = i.color.a * _Color.a * a;

                return fixed4(rgb, alpha);
            }
            ENDCG
        }
    }
}
