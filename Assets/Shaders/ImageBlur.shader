Shader "Image/ImageBlur" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _Size ("Size", Range(0, 20)) = 1
    }
   
        SubShader {
       
            // Horizontal blur
          
            Pass {
               // Tags { "LightMode" = "Always" }
               
                CGPROGRAM
				#pragma vertex vert_img
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"
               
               
                sampler2D _MainTex;
                float4 _MainTex_TexelSize;
                float _Size;
               
                half4 frag( v2f_img i ) : COLOR {
                   
                    half4 sum = half4(0,0,0,0);
 
                    #define GRABPIXEL(weight,kernelx) tex2D( _MainTex, float2(i.uv.x + kernelx*_MainTex_TexelSize.x , i.uv.y))* weight
 
                    sum += GRABPIXEL(0.05, -4.0);
                    sum += GRABPIXEL(0.09, -3.0);
                    sum += GRABPIXEL(0.12, -2.0);
                    sum += GRABPIXEL(0.15, -1.0);
                    sum += GRABPIXEL(0.18,  0.0);
                    sum += GRABPIXEL(0.15, +1.0);
                    sum += GRABPIXEL(0.12, +2.0);
                    sum += GRABPIXEL(0.09, +3.0);
                    sum += GRABPIXEL(0.05, +4.0);

                    return sum;
                }
                ENDCG
            }

        GrabPass {
            Name "PassGrab"
        }
            Pass {
                CGPROGRAM
				#pragma vertex vert_img
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"

                sampler2D _GrabTexture;
                float4 _GrabTexture_TexelSize;
                float _Size;
               
                half4 frag( v2f_img i ) : COLOR {
                   
                    half4 sum = half4(0,0,0,0);
 
                    #define GRABPIXEL(weight,kernely) tex2D( _GrabTexture, float2(i.uv.x, i.uv.y + _GrabTexture_TexelSize.y * kernely)) * weight
 
                    //G(X) = (1/(sqrt(2*PI*deviation*deviation))) * exp(-(x*x / (2*deviation*deviation)))
                   
                    sum += GRABPIXEL(0.05, -4.0);
                    sum += GRABPIXEL(0.09, -3.0);
                    sum += GRABPIXEL(0.12, -2.0);
                    sum += GRABPIXEL(0.15, -1.0);
                    sum += GRABPIXEL(0.18,  0.0);
                    sum += GRABPIXEL(0.15, +1.0);
                    sum += GRABPIXEL(0.12, +2.0);
                    sum += GRABPIXEL(0.09, +3.0);
                    sum += GRABPIXEL(0.05, +4.0);

                  //  sum.r=1;
                    return sum;
                }
                ENDCG
            }
           
        }
}
 