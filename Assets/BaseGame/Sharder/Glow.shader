Shader "UI/ParticleImage/GlowEffect"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        // Glow parameters
        _GlowColor ("Glow Color", Color) = (1, 0.8, 0.3, 1)
        _GlowIntensity ("Glow Intensity", Range(0, 5)) = 1.5
        _GlowSize ("Glow Size", Range(0, 0.1)) = 0.02
        _GlowSamples ("Glow Samples", Range(4, 16)) = 8

        // Animation parameters
        _PulseSpeed ("Pulse Speed", Float) = 2.0
        _PulseMin ("Pulse Min", Range(0, 1)) = 0.5
        _PulseMax ("Pulse Max", Range(1, 3)) = 1.5
        [Toggle] _EnablePulse ("Enable Pulse", Float) = 1

        // Outer glow
        _OuterGlowColor ("Outer Glow Color", Color) = (1, 0.5, 0, 0.5)
        _OuterGlowSize ("Outer Glow Size", Range(0, 0.2)) = 0.05

        // UI Stencil
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;

            fixed4 _GlowColor;
            float _GlowIntensity;
            float _GlowSize;
            float _GlowSamples;
            float _PulseSpeed;
            float _PulseMin;
            float _PulseMax;
            float _EnablePulse;
            fixed4 _OuterGlowColor;
            float _OuterGlowSize;

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.worldPosition = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color * _Color;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Calculate pulse animation
                float pulse = 1.0;
                if (_EnablePulse > 0.5)
                {
                    float t = sin(_Time.y * _PulseSpeed) * 0.5 + 0.5;
                    pulse = lerp(_PulseMin, _PulseMax, t);
                }

                // Sample main texture
                half4 mainColor = (tex2D(_MainTex, i.texcoord) + _TextureSampleAdd) * i.color;

                // Calculate glow by sampling surrounding pixels
                float glowAlpha = 0;
                float samples = _GlowSamples;
                float glowSize = _GlowSize * pulse;

                // Inner glow - sample in circle pattern
                for (float angle = 0; angle < 6.28318; angle += 6.28318 / samples)
                {
                    float2 offset = float2(cos(angle), sin(angle)) * glowSize;
                    glowAlpha += tex2D(_MainTex, i.texcoord + offset).a;
                }
                glowAlpha /= samples;

                // Outer glow - larger radius
                float outerGlowAlpha = 0;
                float outerSize = _OuterGlowSize * pulse;
                for (float angle = 0; angle < 6.28318; angle += 6.28318 / samples)
                {
                    float2 offset = float2(cos(angle), sin(angle)) * outerSize;
                    outerGlowAlpha += tex2D(_MainTex, i.texcoord + offset).a;
                }
                outerGlowAlpha /= samples;

                // Create glow effect
                float innerGlow = saturate(glowAlpha - mainColor.a) * _GlowIntensity * pulse;
                float outerGlow = saturate(outerGlowAlpha - glowAlpha) * pulse;

                // Combine colors
                fixed4 finalColor = mainColor;

                // Add inner glow
                finalColor.rgb += _GlowColor.rgb * innerGlow * _GlowColor.a;

                // Add outer glow behind
                fixed4 outerGlowResult = _OuterGlowColor * outerGlow;

                // Blend outer glow with main
                finalColor.rgb = lerp(outerGlowResult.rgb, finalColor.rgb, mainColor.a + innerGlow);
                finalColor.a = max(mainColor.a, max(innerGlow, outerGlow * _OuterGlowColor.a));

                // Add emission/brightness boost to original image
                finalColor.rgb += mainColor.rgb * (pulse - 1.0) * 0.5 * mainColor.a;

                // UI clipping
                finalColor.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);

                #ifdef UNITY_UI_ALPHACLIP
                clip(finalColor.a - 0.001);
                #endif

                return finalColor;
            }
            ENDCG
        }
    }
}
