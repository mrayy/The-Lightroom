// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Image/ImageMozaik"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" }
		// No culling or depth
		Blend SrcAlpha OneMinusSrcAlpha 
		Cull Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			float _Repeat;

			uniform float4 _MainTex_TexelSize;
			uniform sampler2D _MainTex;
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
				float4 col=tex2Dlod(_MainTex,float4(float2(v.uv.x,v.uv.y),0,1));
				v.vertex.y=col.r*0.5; 
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}



			fixed4 frag (v2f i) : SV_Target
			{
				float4 col;
				float2 uvIn=i.uv;
				//_Position=float4(0.5,0.5,0.6,0);
				float r=_MainTex_TexelSize.x/_MainTex_TexelSize.y;//get render size ratio
				i.uv.y*=r;

				//generate circles pattern
				float2 uv=(i.uv)*_Repeat;
				uv=fmod(uv,1)-0.5;
				float v=sqrt(dot(uv,uv));

				uv=floor(i.uv*_Repeat)/_Repeat;
				v=step(0.4,v);
				if(v>0.5)
					discard;
				col=tex2D(_MainTex,uv);
				col.a=1;

				return col;
			}
			ENDCG
		}
	}
}
