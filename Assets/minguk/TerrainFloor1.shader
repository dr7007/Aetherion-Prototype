Shader "Custom/TransparentTerrain"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {} // 기본 텍스처
        _Transparency ("Transparency", Range(0, 1)) = 1.0 // 투명도
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            sampler2D _MainTex; // 텍스처 샘플러
            half _Transparency; // 투명도 값

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS); // 월드 좌표 변환
                o.uv = v.uv; // UV 매핑
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                texColor.a *= _Transparency; // 알파 값에 투명도 적용
                return texColor;
            }
            ENDHLSL
        }
    }
    FallBack "Transparent/Cutout/VertexLit"
}

