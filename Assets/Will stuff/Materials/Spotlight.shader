Shader "Spotlight"
{
    Properties
    {
        _Color("Overlay Color", Color) = (0,0,0,0.75)
        _Radius("Spot Radius", Float) = 0.2
        _Softness("Edge Softness", Float) = 0.1
    }
    SubShader
    {
        Tags {"Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color;
            float _Radius;
            float _Softness;
            float2 _MousePos; // from script
            float _Aspect;


            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // Correct UV for aspect ratio
                float2 aspectCorrectedUV = float2((uv.x - _MousePos.x) * _Aspect, uv.y - _MousePos.y);
                float dist = length(aspectCorrectedUV);

                float alpha = 1.0 - smoothstep(_Radius, _Radius - _Softness, dist);

                return _Color * alpha;
            }
            ENDCG
        }
    }
}
