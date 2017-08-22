Shader "Hidden/WallRenderShader"
{
    Properties
    {
        _DataTex1   ("-", 2D)       = ""{} //1 zscale,2 SV
        _Hue 		("-",float)		=0
	}
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM

	    #pragma surface surf Lambert vertex:vert finalcolor:mycolor 
	    #pragma target 3.0
	    #pragma glsl

	    #include "UnityCG.cginc"

        struct Input
        {

            float4 color;
        };


        float3 hsv2rgb(float3 c)
		{
		    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
		    float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
		    return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
		}

	    sampler2D _DataTex1;
	    float4 _DataTex_TexelSize;

	    float _Hue;

	    void vert(inout appdata_full v, out Input o)
	    {
          UNITY_INITIALIZE_OUTPUT(Input,o);
	        float2 uv = v.texcoord.xy;

	        float4 p = tex2Dlod(_DataTex1, float4(uv, 0, 0));
	       	v.vertex.z*=p.x;

	        o.color.rgb = hsv2rgb(p.yzw);//_Color * _Options.y;
	        //o.color.a *= sw;

	        //return o; 
	    }

	    void mycolor (Input IN, SurfaceOutput o, inout fixed4 color) {
	    	color=IN.color;
	    }
	    void surf(Input IN, inout SurfaceOutput o)
	    {
	        o.Albedo = 0;
	        o.Alpha = 1;
	    }

        ENDCG
    } 
    Fallback "Diffuse"
}

