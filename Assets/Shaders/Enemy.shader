Shader "CyberSlasher/Enemy" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader {
		Stencil {
			Ref 1
			Comp Always
			Pass replace
		}

		CGPROGRAM
		#pragma surface surf Standard

		fixed4 _Color;

		struct Input {
			fixed4 empty;
		};

		void surf (Input IN, inout SurfaceOutputStandard o) {
			o.Albedo = _Color.rgb;
			o.Alpha = _Color.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
