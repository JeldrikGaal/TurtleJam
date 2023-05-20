Shader "Custom/11 - Volumetric Glow Dissolve"
{
    Properties
    {
        [HDR] _Color ("Color", Color) = (1,1,1,1)
        [HDR] _EdgeColor ("EdgeColor", Color) = (1,1,1,1)
        _EdgeSize ("Edge Size", float) = 0.03

        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "bump" {}
        _Smoothness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.5
        _Clip("Alpha Clip", Range(0, 1)) = 1
        _NoiseScale("Noise Scale", float) = 1
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200
            Cull Off

            CGPROGRAM
            #pragma surface surf Standard
            #pragma target 3.0
            #include "noiseSimplex.cginc"

            sampler2D _MainTex;
            sampler2D _NormalMap;
            fixed4 _Color;
            half _Smoothness;
            half _Metallic;
            float _Clip;
            float _NoiseScale;
            fixed4 _EdgeColor;
            float _EdgeSize;

            struct Input
            {
                float2 uv_MainTex;
                float2 uv_NormalMap;
                float3 worldPos;
            };

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                float noise = snoise(IN.worldPos * _NoiseScale) * 0.5 + 0.5;
                float threshold = noise - _Clip + _EdgeSize;

                if(threshold > _EdgeSize)
                {
                    noise += snoise(float4(IN.worldPos * _NoiseScale * 5, _Time.y)) * 0.1;
                }
                
                if (noise > _Clip)
                    discard;
                
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
                o.Albedo = c.rgb;
                o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap));
                o.Alpha = c.a;
                o.Metallic = _Metallic;
                o.Smoothness = _Smoothness;
                o.Emission = lerp(fixed3(0,0,0), _EdgeColor.rgb ,step(0, threshold));
            }
            ENDCG
        }
            FallBack "Diffuse"
}