
Shader "Custom/DustyMirror"
{
    Properties
    {
        _BaseTex ("Mirror Base", 2D) = "gray" {}
        _DustTex ("Dust Texture", 2D) = "black" {}

        _DustStrength ("Dust Strength", Range(0, 1)) = 0.5

        _DustColor ("Dust Color", Color) = (0.12, 0.12, 0.12, 1)

        _Smoothness ("Smoothness", Range(0, 1)) = 0.7
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "Queue"="Geometry"
        }

        LOD 200

        CGPROGRAM

        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _BaseTex;
        sampler2D _DustTex;

        fixed _DustStrength;
        fixed4 _DustColor;
        half _Smoothness;

        struct Input
        {
            float2 uv_BaseTex;
            float2 uv_DustTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Основной цвет зеркала
            fixed4 baseColor = tex2D(_BaseTex, IN.uv_BaseTex);

            // Читаем пыль в оттенках серого
            fixed4 dust = tex2D(_DustTex, IN.uv_DustTex);

            // Чёрный фон = 0, светлые пятна = большее значение
            fixed mask = dot(dust.rgb, fixed3(0.299, 0.587, 0.114));

            // Регулируем интенсивность загрязнений
            mask = saturate(mask * _DustStrength);

            // Накладываем тёмную грязь поверх зеркала
            o.Albedo = lerp(baseColor.rgb, _DustColor.rgb, mask);

            o.Metallic = 0;
            o.Smoothness = _Smoothness;
            o.Alpha = 1;
        }

        ENDCG
    }

    FallBack "Standard"
}
