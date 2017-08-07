Shader "Hidden/GazeMaskShader"
{
	SubShader
	{
		Tags { "RenderType"="Transparent" }
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}

			float2 _ScreenSize;
			float4 _Parameters;

			fixed4 frag (v2f i) : SV_Target
			{
			//	fixed4 m = tex2D(_MainTex, i.uv);
				float4 col=float4(0,0,0,1);
				float4 blur=float4(1,1,1,1);

				float2 uv=i.uv*_ScreenSize;

				float r1=_Parameters.z;
				float r2=r1+_Parameters.w*1000;


				float dist=dot(uv-_Parameters.xy,uv-_Parameters.xy);
				if(dist<r1*r1)
					return col;
				if(dist>r2*r2)
					return blur;

				dist=sqrt(dist);
				float t=(dist-r1)/(r2-r1);

				return lerp(col,blur,t); 
			}
			ENDCG
		}
	}
}
