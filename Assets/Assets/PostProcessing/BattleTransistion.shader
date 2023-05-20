Shader "Hidden/BattleTransistion"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Transition("Transition", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off 
        ZWrite Off 
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.uv1 = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _Transition;
            float _Cutoff;
            fixed4 _Color;

            fixed4 frag (v2f i) : SV_Target
            {

            
                fixed4 transition = tex2D(_Transition, i.uv1);
                if (transition.b < _Cutoff)
                    return _Color;

                return tex2D(_MainTex,  i.uv);
            }
            ENDCG
        }
    }
}
