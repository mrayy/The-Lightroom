Shader "Objects/ListenerShader"
{
    Properties
    {
    	_Cutoff("Cutoff",float)=0.5
    	_Exp("Exp",float)=0.5
    	_Color1("Color",Color)=(1,1,1,1)
    	_Color2("Color",Color)=(1,1,1,1)
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

          	float3 viewDir;
            float4 color;
        };


        float3 hsv2rgb(float3 c)
		{
		    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
		    float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
		    return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
		}

	    float4 _Color1;
	    float4 _Color2;
	    float _Exp;
	    float _Cutoff;

	    void vert(inout appdata_full v, out Input o)
	    {
          UNITY_INITIALIZE_OUTPUT(Input,o);
	    }

	    void mycolor (Input IN, SurfaceOutput o, inout fixed4 color) {
	    	float d=1-dot(normalize(IN.viewDir),normalize(o.Normal));
	    	d=pow(d,_Exp);
	    	d=d*(1-_Cutoff)+_Cutoff;
	    	color=lerp(_Color1,_Color2,d);
	    }
	    void surf(Input IN, inout SurfaceOutput o)
	    {
	        o.Emission = 1;
	        o.Alpha = 1;
	    }

        ENDCG
    } 
    Fallback "Diffuse"
}

