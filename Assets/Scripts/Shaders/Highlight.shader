Shader "UI/Highlight"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _USize ("U Size", Range(.0, 1.)) = 0.1
        _Range ("Range", Range(0.0, 1.0)) = 0
        
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15
        
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha 
        LOD 100
        ColorMask [_ColorMask]

        
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            fixed _USize;
            fixed _Range;
            sampler2D _MainTex;
            fixed4 _TextureSampleAdd;
            fixed4 _MainTex_ST;
            
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP
            
            struct appdata
            {
                fixed4 vertex : POSITION;
                fixed4 color    : COLOR;
                fixed2 uv : TEXCOORD0;
            };

            struct v2f
            {
                fixed2 uv : TEXCOORD0;
                fixed4 color    : COLOR;
                fixed4 vertex : SV_POSITION;
                fixed4 worldPosition : TEXCOORD1;
            };


            v2f vert (appdata v)
            {
                v2f o;
                o.worldPosition = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color = (tex2D(_MainTex, i.uv) + _TextureSampleAdd) * i.color;
                fixed Alpha = (abs(i.uv.x - _Range*(1+_USize*2) + _USize)) / _USize;
    #ifdef UNITY_UI_ALPHACLIP
                clip(1-Alpha-0.01f);
    #endif
                color.a -=  Alpha;
                return color;
            }
            ENDCG
        }
    }
}
