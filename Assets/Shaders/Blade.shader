Shader "CyberSlasher/Blade" {
Properties {
    _MainTex ("Cut Texture", 2D) = "white" {}
    _CutColor ("Cut Color", Color) = (1,1,1,1)
    _Color ("Main Color", Color) = (1,1,1,1)
    _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	_Sharpness("Sharpness", Range(0,1)) = 1
	_OverlapColor("Overlap Color", Color) = (1, 1, 1, 1)
}
SubShader {
    Tags {"Queue"="Geometry+1"}

    Lighting Off

    Pass {
		Stencil {
			Ref 1
			Comp NotEqual
			Pass Keep
		}

        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            half _Cutoff;
			half _Sharpness;
			fixed4 _Color;
			fixed4 _CutColor;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
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

	Pass {
		Stencil {
			Ref 1
			Comp Equal
			Pass Replace
		}
		ZTest Always

		CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
            };

            fixed4 _OverlapColor;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _OverlapColor;
            }
        ENDCG
    }
}
FallBack "Diffuse"
}
