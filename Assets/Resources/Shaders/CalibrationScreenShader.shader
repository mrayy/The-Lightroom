// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/CalibrationScreenShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
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
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}


			float _Repeat;

			float4 _Position;
			uniform float4 _MainTex_TexelSize;
			uniform sampler2D _MainTex;

			fixed4 frag (v2f i) : SV_Target
			{
				float4 col;
				
				float2 uvIn=i.uv;
				//_Position=float4(0.5,0.5,0.6,0);
				float r=_MainTex_TexelSize.x/_MainTex_TexelSize.y;//get render size ratio
				i.uv.y*=r;
				_Position.y*=r;

				//generate circles pattern
				float2 uv=(i.uv)*_Repeat;
				uv=fmod(uv,1)-0.5;
				float v=sqrt(dot(uv,uv));

				//calculate pointing area
				float dist=length(i.uv-_Position.xy)/_Position.w;
				dist=pow(dist,3);
				dist=clamp(0,1,dist);
				float cut=lerp(0.4,_Position.z,dist);

				v=step(cut,v);
				//col=lerp(tex2D(_BGTex,uvIn),tex2D(_Tex,uvIn),1-v);
				col=float4(1,1,1,1-v);
				return col;
			}
			ENDCG
		}
	}
}
