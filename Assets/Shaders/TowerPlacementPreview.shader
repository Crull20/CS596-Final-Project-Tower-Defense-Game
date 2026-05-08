Shader "Custom/URP/TowerPlacementPreview"
{
    Properties
    {
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        [MainColor] _ValidColor("Valid Color", Color) = (0.2, 1.0, 0.25, 1.0)
        _InvalidColor("Invalid Color", Color) = (1.0, 0.2, 0.2, 1.0)

        _PlacementValid("Placement Valid", Range(0,1)) = 1
        _BaseAlpha("Base Alpha", Range(0,1)) = 0.45

        _FresnelPower("Fresnel Power", Range(0.5, 8.0)) = 3.0
        _EdgeStrength("Edge Strength", Range(0, 2.0)) = 0.6

        _PulseSpeed("Pulse Speed", Range(0, 10.0)) = 2.0
        _PulseStrength("Pulse Strength", Range(0, 1.0)) = 0.12
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            Name "ForwardUnlit"

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                half3 normalWS    : TEXCOORD1;
                float3 viewDirWS  : TEXCOORD2;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _ValidColor;
                half4 _InvalidColor;
                half  _PlacementValid;
                half  _BaseAlpha;
                half  _FresnelPower;
                half  _EdgeStrength;
                half  _PulseSpeed;
                half  _PulseStrength;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                VertexPositionInputs posInputs = GetVertexPositionInputs(IN.positionOS.xyz);

                OUT.positionCS = posInputs.positionCS;
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewDirWS = GetCameraPositionWS() - posInputs.positionWS;

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 texCol = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);

                half4 tint = lerp(_InvalidColor, _ValidColor, saturate(_PlacementValid));

                half3 normalWS = normalize(IN.normalWS);
                half3 viewDirWS = normalize((half3)IN.viewDirWS);

                half fresnel = 1.0h - saturate(dot(normalWS, viewDirWS));
                fresnel = pow(fresnel, _FresnelPower);

                half pulse = (sin(_Time.y * _PulseSpeed) * 0.5h + 0.5h) * _PulseStrength;

                half3 finalRgb = texCol.rgb * tint.rgb;
                finalRgb += tint.rgb * fresnel * _EdgeStrength;
                finalRgb += tint.rgb * pulse;

                half finalAlpha = saturate(_BaseAlpha * texCol.a + fresnel * 0.25h);

                return half4(finalRgb, finalAlpha);
            }
            ENDHLSL
        }
    }
}