Shader "Hidden/ParticleRenderShader"
{
    Properties
    {
        _DataTex1   ("-", 2D)       = ""{} //3pos,1 zscale
        _DataTex2   ("-", 2D)       = ""{} //3rot,1 lifespan
        _Hue 		("-",float)		=0
	}
    CGINCLUDE

	  

	    #include "UnityCG.cginc"

	    struct appdata
	    {
	        float4 pos : POSITION;
	        float2 texcoord : TEXCOORD0;
	    };

	    struct v2f
	    {
	        float4 pos : SV_POSITION;
	        float4 color : COLOR;
	    };



        float3 hsv2rgb(float3 c)
		{
		    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
		    float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
		    return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
		}
        // Quaternion multiplication.
        // http://mathworld.wolfram.com/Quaternion.html
        float4 qmul(float4 q1, float4 q2)
        {
            return float4(
                q2.xyz * q1.w + q1.xyz * q2.w + cross(q1.xyz, q2.xyz),
                q1.w * q2.w - dot(q1.xyz, q2.xyz)
            );
        }

        // Rotate a vector with a rotation quaternion.
        // http://mathworld.wolfram.com/Quaternion.html
        float3 rotateVec(float3 v, float4 r)
        {
            float4 r_c = r * float4(-1, -1, -1, 1);
            return qmul(r, qmul(float4(v, 0), r_c)).xyz;
        }

	    sampler2D _DataTex1;
	    sampler2D _DataTex2;

	    float _Hue;
	    float _Saturation;

    	v2f vert(appdata v)
	    {
       		v2f o;

	        float2 uv = v.texcoord.xy;

	        float4 p = tex2Dlod(_DataTex1, float4(uv, 0, 0));
	        float4 r = tex2Dlod(_DataTex2, float4(uv, 0, 0));

	        float4 rot;
	        rot.xyz=r.xyz;
	        rot.w=sqrt(1.0-dot(rot.xyz,rot.xyz));

	        o.pos=UnityObjectToClipPos(float4( rotateVec(v.pos.xyz,rot)*p.w+p.xyz,1));
	        //v.normal.xyz=rotateVec(v.normal.xyz,rot);

	        o.color.rgb = hsv2rgb(float3(_Hue,_Saturation,r.w*0.5+0.5));//_Color * _Options.y;
	        o.color.a = 1-r.w;

	        return o; 
	    }

	    float4 frag (v2f IN): COLOR 
	    {
	    	return IN.color;
	    }

        ENDCG

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

        Pass
        {
         Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
         //  #pragma surface surf Lambert vertex:vert finalcolor:mycolor 
        #pragma vertex vert
        #pragma fragment frag
	    #pragma target 3.0
	    #pragma glsl
        ENDCG
        }
    } 
}

