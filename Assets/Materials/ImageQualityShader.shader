
Shader "Custom/ImageQualityShader" {
    Properties{
        _MainTex("Texture", 2D) = "black" { }
        _Brightness("Brightness", Range(-1, 1)) = 0
        _Contrast("Contrast", Range(-2 , 4)) = 1
        _Sharpness("Sharpness", Range(0 , 15)) = 0
        _Test("Test", Range(1,1024)) = 1
    }
        SubShader{
            Pass {

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                sampler2D _MainTex;
                float _Brightness;
                float _Contrast;
                float _Sharpness;
                float _Test;

                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;

                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct v2f {
                    float4  pos : SV_POSITION;
                    float2  uv : TEXCOORD0;

                    UNITY_VERTEX_INPUT_INSTANCE_ID
                    UNITY_VERTEX_OUTPUT_STEREO
                };

                float4 _MainTex_ST;

                v2f vert(appdata_base v)
                {
                    v2f o;

                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_TRANSFER_INSTANCE_ID(v, o);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);



                    return o;
                }

                // returns sharpened pixel at given
                float sharpenFilter(v2f i)
                {

                    float3 f =
                        tex2D(_MainTex, i.uv + float2(-1.0/_Test, -1.0/_Test)).r * -1. +
                        tex2D(_MainTex, i.uv + float2(0, -1.0/_Test)).r * -1. +
                        tex2D(_MainTex, i.uv + float2(1.0/_Test, -1.0/_Test)).r * -1. +
                        tex2D(_MainTex, i.uv + float2(-1.0/_Test, 0)).r * -1. +
                        tex2D(_MainTex, i.uv + float2(0, 0)).r * _Sharpness +
                        tex2D(_MainTex, i.uv + float2(1.0/_Test, 0)).r * -1. +
                        tex2D(_MainTex, i.uv + float2(-1.0/_Test, 1.0/_Test)).r * -1. +
                        tex2D(_MainTex, i.uv + float2(0, 1.0/_Test)).r * -1. +
                        tex2D(_MainTex, i.uv + float2(1.0/_Test, 1.0/_Test)).r * -1.;
   

                    return f;


                }


                fixed4 frag(v2f i) : COLOR
                {
                    UNITY_SETUP_INSTANCE_ID(i);

                    // convert from nv12 to rgba
                    //float texcol = tex2D(_MainTex, i.uv).r;

                    //sharpen pixel
                    //float val = sharpenFilter(i.uv);  // Adjust strength as needed
                    //float val = tex2D(_MainTex, i.uv);  // Adjust strength as needed

                    float val = sharpenFilter(i);

                    fixed4 color = fixed4(val, val, val, 1.0f);
                    color.rgb += _Brightness;
                    color.rgb = (color.rgb - 0.5) * _Contrast + 0.5;


                    //tex2D(_MainTex, i.uv).rgb = color;

                    return color;
                    //return tex2D(_MainTex, i.uv).rgba;
                }

                ENDCG

            }
        }
            Fallback "VertexLit"
}