Shader "Unlit/Unlit Colored Texture"
{
    Properties
    {
        _Mask ("Texture", 2D) = "white" {}
        _MaskColor("Mask Color", Color) = (1,1,1,1)
    }
    SubShader
    { 
        Tags {"Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha 
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 vertex : SV_POSITION;
            };

            sampler2D _Mask;
            fixed4 _Mask_ST;
            fixed4 _MaskColor;

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.uv = TRANSFORM_TEX(v.uv, _Mask);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed Lerp = tex2D(_Mask, i.uv).r;
                clip (Lerp -0.01f);
                return _MaskColor;
            }
            ENDCG
        }
    }
}
