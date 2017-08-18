// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Mat" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Color (RGB)", Color) = (1, 1, 1, 1)
	}
	SubShader {
		Pass {
			CGPROGRAM
	
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _Color;

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			float4 _MainTex_ST;

			v2f vert(appdata_base v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : COLOR0 {
			    half4 texcol = tex2D (_MainTex, i.uv);
			    return texcol * _Color;
			}
			ENDCG
		}
	} 
}
