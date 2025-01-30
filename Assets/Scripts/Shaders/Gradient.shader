Shader "UI/Gradient"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _UpLeft ("Up Left Color", Color) = (0,0,0,1)
        _UpRight ("Up Right Color", Color) = (0,0,0,1)
        _DownLeft ("Down Left Color", Color) = (0,0,0,1)
        _DonwRight ("Down Right Color", Color) = (0,0,0,1)
        
        
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15
    }
    SubShader
    {
        //Tags {"Queue"="Transparent" "RenderType"="Transparent" }
        //Blend SrcAlpha OneMinusSrcAlpha
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

            struct appdata
            {
                fixed4 vertex : POSITION;
                fixed2 uv : TEXCOORD0;
            };

            struct v2f
            {
                fixed2 uv : TEXCOORD0;
                fixed4 vertex : SV_POSITION;
                //fixed4 upColor : COLOR0;
                //fixed3 downColor : COLOR1;
            };
            fixed4 _UpLeft;
            fixed4 _UpRight;
            fixed4 _DownLeft;
            fixed4 _DonwRight;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                //o.upColor.rgb = lerp(_UpLeft, _UpRight, v.uv.x).rgb;
                //o.upColor.a = v.uv.y;
                //o.downColor = lerp(_DownLeft, _DonwRight, v.uv.x).rgb;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //return fixed4(lerp(i.downColor, i.upColor, i.upColor.a), 1);// i.color;
                
                fixed4 UpColor = lerp(_UpLeft, _UpRight, i.uv.x);
                fixed4 DownColor = lerp(_DownLeft, _DonwRight, i.uv.x);
                return lerp(DownColor, UpColor, i.uv.y);
                
            }
            ENDCG
        }
    }
}
