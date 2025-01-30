Shader "Unlit/BubbleShader"
{
    Properties
    {
        _MainTex ("Base", 2D) = "white" {}
        _DarkTex ("Dark", 2D) = "black" {}
        _Dark_Sense("Dark Sense", Range(.0, 1.)) = 1
        _OverlayTex ("Overlay", 2D) = "black" {}
        _Overlay_Sense("Overlay Sense", Range(.0, 1.)) = 1
        _Overlay2Tex ("Overlay2", 2D) = "black" {}
        _Overlay2_Sense("Overlay2 Sense", Range(.0, 1.)) = 1
        _Shift("Shift", Float) = 1
    }
    SubShader
    {
        Blend SrcAlpha OneMinusSrcAlpha 
        LOD 100
        Pass
        {
            Tags {"Queue"="Transparent+1" "RenderType"="Transparent" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                fixed2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            fixed4 _MainTex_ST;
            sampler2D _DarkTex;
            fixed _Dark_Sense;
            sampler2D _OverlayTex;
            fixed _Overlay_Sense;
            sampler2D _Overlay2Tex;
            fixed _Overlay2_Sense;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                clip(col.a-0.1);
                fixed4 dark = tex2D(_DarkTex, i.uv);
                fixed4 overlay = tex2D(_OverlayTex, i.uv);
                fixed4 overlay2 = tex2D(_Overlay2Tex, i.uv);
                //mask *= 0.7f + 0.3f * (_SinTime.w / 2 + 0.5f);
                fixed4 Result = col * lerp(dark, 1, _Dark_Sense) + overlay *_Overlay_Sense  + overlay2 *_Overlay2_Sense;
                Result = fixed4(Result.rgb, i.color.a);
                return Result;
            }
            ENDCG
        }
    }
}
