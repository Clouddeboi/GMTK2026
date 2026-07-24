Shader "Custom/SimpleToon"
{
    Properties
    {
        _BaseMap ("Texture", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1,1,1,1)

        _ShadowColor ("Shadow Color", Color) = (0.35,0.35,0.35,1)
        _ShadowThreshold ("Shadow Threshold", Range(0,1)) = 0.5
        _ShadowSmoothness ("Shadow Smoothness", Range(0.001,0.5)) = 0.05

        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0,0.05)) = 0.01
    }


    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "RenderPipeline"="UniversalPipeline"
        }

        //OUTLINE PASS

        Pass
        {
            Name "Outline"

            Cull Front
            ZWrite On

            HLSLPROGRAM

            #pragma vertex OutlineVertex
            #pragma fragment OutlineFragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"


            float4 _OutlineColor;
            float _OutlineWidth;


            struct OutlineAttributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };


            struct OutlineVaryings
            {
                float4 positionCS : SV_POSITION;
            };


            OutlineVaryings OutlineVertex(OutlineAttributes input)
            {
                OutlineVaryings output;


                float3 positionOS = input.positionOS.xyz;

                positionOS += normalize(input.normalOS) * _OutlineWidth;


                output.positionCS =
                    TransformObjectToHClip(positionOS);


                return output;
            }


            half4 OutlineFragment(OutlineVaryings input)
                : SV_Target
            {
                return _OutlineColor;
            }


            ENDHLSL
        }

        //TOON FORWARD PASS

        Pass
        {
            Name "ForwardLit"

            Tags
            {
                "LightMode"="UniversalForward"
            }


            HLSLPROGRAM


            #pragma vertex ToonVertex
            #pragma fragment ToonFragment


            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT


            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"



            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);


            CBUFFER_START(UnityPerMaterial)

                float4 _BaseColor;

                float4 _ShadowColor;

                float _ShadowThreshold;
                float _ShadowSmoothness;

            CBUFFER_END



            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };



            struct Varyings
            {
                float4 positionCS : SV_POSITION;

                float3 normalWS : TEXCOORD0;

                float2 uv : TEXCOORD1;

                float3 positionWS : TEXCOORD2;
            };



            Varyings ToonVertex(Attributes input)
            {
                Varyings output;


                output.positionCS =
                    TransformObjectToHClip(input.positionOS.xyz);


                output.normalWS =
                    TransformObjectToWorldNormal(input.normalOS);


                output.positionWS =
                    TransformObjectToWorld(input.positionOS.xyz);


                output.uv = input.uv;


                return output;
            }




            half4 ToonFragment(Varyings input)
                : SV_Target
            {


                float3 normal =
                    normalize(input.normalWS);



                Light mainLight =
                    GetMainLight();



                float NdotL =
                    dot(normal, mainLight.direction);



                float shadowValue =
                    smoothstep(
                        _ShadowThreshold - _ShadowSmoothness,
                        _ShadowThreshold + _ShadowSmoothness,
                        NdotL
                    );



                float4 tex =
                    SAMPLE_TEXTURE2D(
                        _BaseMap,
                        sampler_BaseMap,
                        input.uv
                    );



                float3 baseColor =
                    tex.rgb * _BaseColor.rgb;



                float3 toonLighting =
                    lerp(
                        _ShadowColor.rgb,
                        1,
                        shadowValue
                    );



                float3 finalColor =
                    baseColor * toonLighting;



                //Apply light colour

                finalColor *= mainLight.color;



                return float4(finalColor,1);

            }


            ENDHLSL
        }
    }
}