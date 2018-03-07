Shader "CyberSlasher/Blade" {
Properties {
    _MainTex ("Cut Texture", 2D) = "white" {}
    _CutColor ("Cut Color", Color) = (1,1,1,1)
    _Color ("Main Color", Color) = (1,1,1,1)
    _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	_Sharpness("Sharpness", Range(0,1)) = 1
}
SubShader {
    Tags {"Queue"="Opaque"}
    LOD 100

    Lighting Off

    Pass {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                UNITY_VERTEX_OUTPUT_STEREO
            };

            fixed _Cutoff;
			fixed _Sharpness;
			fixed4 _Color;
			fixed4 _CutColor;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed a = tex2D(_MainTex, i.texcoord).a;
				fixed visibility = clamp((a - _Cutoff) * (256 * _Sharpness), 0, 1);
                fixed4 col = lerp(_CutColor, _Color, visibility);
                return col;
            }
        ENDCG
    }
}

}
