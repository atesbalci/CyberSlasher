Shader "CyberSlasher/Ground"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_SpectrumTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float2 colorUv : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _SpectrumTex;
			float4 _SpectrumTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.colorUv = float2(fmod(v.vertex.x, 1.0), 1.0);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return tex2D(_MainTex, i.uv) * tex2D(_SpectrumTex, i.colorUv);
			}
			ENDCG
		}
	}
}
