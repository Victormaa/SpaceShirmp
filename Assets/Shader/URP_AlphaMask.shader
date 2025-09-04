Shader "FxClass/URP_AlphaMask"
{
    Properties
    {
        _AlphaIntensity("Alpha强度", Float) = 0
        [HDR]_MainColor("主色调", Color) = (0.6792453,0.6792453,0.6792453,0)
        _MainTex("主贴图", 2D) = "white" {}
        _MainTexUspeed("主贴图U速度", Float) = 0
        _MianTexVspeed("主贴图V速度", Float) = 0
        _SecondTex("纹理贴图", 2D) = "white" {}
        _SecTexUspeed("纹理贴图U速度", Float) = 0
        _SecTexVspeed("纹理贴图V速度", Float) = 0
        _MaskTex("遮罩贴图", 2D) = "white" {}
        _Softedge("软粒子", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float3 positionWS : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_SecondTex);
            SAMPLER(sampler_SecondTex);
            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _SecondTex_ST;
                float4 _MaskTex_ST;
                float4 _MainColor;
                float _MainTexUspeed;
                float _MianTexVspeed;
                float _SecTexUspeed;
                float _SecTexVspeed;
                float _AlphaIntensity;
                float _Softedge;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.color = IN.color;
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.screenPos = ComputeScreenPos(OUT.positionHCS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // 计算UV动画
                float2 mainUV = TRANSFORM_TEX(IN.uv, _MainTex);
                float2 secondUV = TRANSFORM_TEX(IN.uv, _SecondTex);
                float2 maskUV = TRANSFORM_TEX(IN.uv, _MaskTex);

                float2 mainPanner = mainUV + float2(_MainTexUspeed, _MianTexVspeed) * _Time.y;
                float2 secondPanner = secondUV + float2(_SecTexUspeed, _SecTexVspeed) * _Time.y;

                // 采样纹理
                half mainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, mainPanner).r;
                half secondTex = SAMPLE_TEXTURE2D(_SecondTex, sampler_SecondTex, secondPanner).r;
                half maskTex = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, maskUV).r;

                // 计算软粒子
                float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
                float sceneDepth = LinearEyeDepth(SampleSceneDepth(screenUV), _ZBufferParams);
                float surfaceDepth = LinearEyeDepth(IN.positionHCS.z, _ZBufferParams);
                float depthFade = saturate(abs(sceneDepth - surfaceDepth) / _Softedge);

                // 合成最终颜色和透明度
                half3 emission = IN.color.rgb * _MainColor.rgb * secondTex;
                half alpha = mainTex * maskTex * secondTex * IN.color.a * _AlphaIntensity * depthFade;

                return half4(emission, saturate(alpha));
            }
            ENDHLSL
        }
    }
}