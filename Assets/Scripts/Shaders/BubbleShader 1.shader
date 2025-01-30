Shader "Unlit/BubbleShader1"
{
    Properties
    {
        _MainTex ("Base", 2D) = "white" {}
        _DarkTex ("Dark", 2D) = "white" {}
        _OverlayTex ("Overlay", 2D) = "black" {}
        _ClampSize("Clamp Size", Range(.0, 1.)) = 0
    }
    SubShader
    {
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            Tags {"Queue"="Transparent+1" "RenderType"="Transparent" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                half4 vertex : POSITION;
                fixed2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                half4 vertex : SV_POSITION;
                fixed2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            fixed _ClampSize;
            sampler2D_half _MainTex;
            sampler2D_half _DarkTex;
            sampler2D_half _OverlayTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                clip(col.a-_ClampSize);
                fixed4 dark = tex2D(_DarkTex, i.uv);
                fixed4 overlay = tex2D(_OverlayTex, i.uv);
                fixed4 Result = col * dark + overlay;
                Result = fixed4(Result.rgb, i.color.a);
                return Result;
            }
            ENDCG
        }
    }
}
