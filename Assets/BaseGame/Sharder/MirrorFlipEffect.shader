Shader "Custom/ShineEffect"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _ShineColor ("Shine Color", Color) = (1, 1, 1, 0.8)
        _ShineWidth ("Shine Width", Range(0.01, 0.5)) = 0.15
        _ShineSpeed ("Shine Speed", Float) = 1.0
        _ShineAngle ("Shine Angle", Range(0, 360)) = 45
        _ShineIntensity ("Shine Intensity", Range(0, 3)) = 1.5

        [Toggle] _EnableShine ("Enable Shine", Float) = 1
        [Toggle] _Loop ("Loop Animation", Float) = 1
        _Delay ("Delay Between Loops", Float) = 0.5

        [PerRendererData] _StartTime ("Start Time", Float) = 0

        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)

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
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "Default"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #pragma multi_compile_instancing
            #pragma multi_compile _ UNITY_UI_CLIP_RECT
            #pragma multi_compile _ UNITY_UI_ALPHACLIP

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
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _RendererColor;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;

            // Non-instanced properties
            fixed4 _ShineColor;
            float _ShineWidth;
            float _ShineSpeed;
            float _ShineAngle;
            float _ShineIntensity;
            float _EnableShine;
            float _Loop;
            float _Delay;

            // Per-instance properties
            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float, _StartTime)
            UNITY_INSTANCING_BUFFER_END(Props)

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.worldPosition = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color * _Color * _RendererColor;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                half4 mainColor = (tex2D(_MainTex, i.texcoord) + _TextureSampleAdd) * i.color;

                float shine = 0;
                if (_EnableShine > 0.5)
                {
                    float angleRad = _ShineAngle * 0.01745329;

                    float2 centeredUV = i.texcoord - 0.5;
                    float rotatedPos = centeredUV.x * cos(angleRad) + centeredUV.y * sin(angleRad);
                    rotatedPos += 0.5;

                    // Get per-instance start time
                    float startTime = UNITY_ACCESS_INSTANCED_PROP(Props, _StartTime);
                    float elapsedTime = max(0, _Time.y - startTime);
                    
                    float cycleTime = 1.0 + _Delay;
                    float time = elapsedTime * _ShineSpeed;

                    float shinePos;
                    if (_Loop > 0.5)
                    {
                        shinePos = frac(time / cycleTime) * (1.0 + _ShineWidth * 2) - _ShineWidth;
                    }
                    else
                    {
                        shinePos = saturate(time / cycleTime) * (1.0 + _ShineWidth * 2) - _ShineWidth;
                    }

                    float dist = abs(rotatedPos - shinePos);
                    shine = 1.0 - saturate(dist / _ShineWidth);
                    shine = pow(shine, 2.0);
                    shine *= _ShineIntensity * mainColor.a;
                }

                fixed4 finalColor = mainColor;
                finalColor.rgb += _ShineColor.rgb * shine * _ShineColor.a;

                #ifdef UNITY_UI_CLIP_RECT
                finalColor.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(finalColor.a - 0.001);
                #endif

                return finalColor;
            }
            ENDCG
        }
    }

    Fallback "Sprites/Default"
}
