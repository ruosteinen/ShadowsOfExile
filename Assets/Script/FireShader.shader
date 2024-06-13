Shader "Custom/FireShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _NoiseTex ("Noise (RGB)", 2D) = "white" {}
        _Color ("Main Color", Color) = (1, 1, 1, 1)
        _Speed ("Speed", Range(0, 10)) = 1.0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Speed;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 mainTex = tex2D(_MainTex, i.uv);
                half4 noiseTex = tex2D(_NoiseTex, i.uv * _Speed + _Time.y * _Speed);

                half alpha = noiseTex.r;
                half4 color = _Color * mainTex;
                color.a = alpha;

                return color;
            }
            ENDCG
        }
    }
}