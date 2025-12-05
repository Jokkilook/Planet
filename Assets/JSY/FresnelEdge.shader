Shader "Unlit/FresnelEdge"
{
    Properties
    {
        _Color ("Edge Color", Color) = (1,1,0,1)
        _Power ("Fresnel Power", Range(0.1, 10)) = 3
        _Intensity ("Edge Intensity", Range(0,5)) = 1
        _Alpha ("Alpha", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back

        Pass
        {
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // VR + Instancing 지원
            #pragma multi_compile_instancing
            #pragma multi_compile _ UNITY_SINGLE_PASS_STEREO
            #include "UnityCG.cginc"
            #include "UnityInstancing.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normalDir : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 _Color;
            float _Power;
            float _Intensity;
            float _Alpha;

            v2f vert(appdata v)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                v2f o;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(_WorldSpaceCameraPos - worldPos);

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float fresnel = pow(1.0 - saturate(dot(normalize(i.normalDir), normalize(i.viewDir))), _Power);
                float edgeAlpha = fresnel * _Intensity * _Alpha;

                return float4(_Color.rgb, edgeAlpha);
            }

            ENDHLSL
        }
    }
}
