Shader "Custom/TransparentTerrain"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {} // �⺻ �ؽ�ó
        _Transparency ("Transparency", Range(0, 1)) = 1.0 // ����
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

            sampler2D _MainTex; // �ؽ�ó ���÷�
            half _Transparency; // ���� ��

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
                o.positionCS = TransformObjectToHClip(v.positionOS); // ���� ��ǥ ��ȯ
                o.uv = v.uv; // UV ����
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                texColor.a *= _Transparency; // ���� ���� ���� ����
                return texColor;
            }
            ENDHLSL
        }
    }
    FallBack "Transparent/Cutout/VertexLit"
}

