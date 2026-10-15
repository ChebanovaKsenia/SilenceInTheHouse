Shader "Custom/GlassShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _AlphaMap ("Alpha Map", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
        
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        
        CGPROGRAM
        #pragma surface surf Standard alpha
        
        sampler2D _MainTex;
        sampler2D _AlphaMap;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        
        struct Input
        {
            float2 uv_MainTex;
        };
        
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            fixed4 a = tex2D (_AlphaMap, IN.uv_MainTex);
            
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = a.r; // Берём прозрачность из красного канала AlphaMap
        }
        ENDCG
    }
    FallBack "Diffuse"
}