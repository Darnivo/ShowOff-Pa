Shader "UI/MultiSpotlight"
{
    Properties
    {
        _Color("Overlay Color", Color) = (0,0,0,0.75)
        _Radius("Default Spotlight Radius", Float) = 0.2
        _Softness("Default Edge Softness", Float) = 0.05
        _Intensity("Default Light Intensity", Float) = 1.0
        [HideInInspector]_SpotCount("Spot Count", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Overlay" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color;
            float _Radius;
            float _Softness;
            float _Intensity;
            float _Aspect;
            int _SpotCount;
            float4 _SpotPositions[10];
            float _SpotRadii[10];      
            float _SpotSoftness[10];   
            float _SpotIntensities[10]; // Individual intensity for each spot

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float lightSum = 0.0;

                for (int n = 0; n < _SpotCount; n++) {
                    float2 diff = uv - _SpotPositions[n].xy;
                    diff.x *= _Aspect;
                    float dist = length(diff);
                    
                    // Use individual radius, softness, and intensity for each spotlight
                    float radius = _SpotRadii[n];
                    float softness = _SpotSoftness[n];
                    float intensity = _SpotIntensities[n];
                    
                    float light = 1.0 - smoothstep(radius, radius + softness, dist);
                    light *= intensity; // Apply individual intensity
                    lightSum = max(lightSum, light);
                }

                float alpha = saturate(1.0 - lightSum);
                fixed4 col = _Color;
                col.a *= alpha;
                return col;
            }

            ENDCG
        }
    }
}