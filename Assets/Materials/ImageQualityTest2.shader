Shader "Custom/EdgeDetectionShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Brightness("Brightness", Range(-1, 1)) = 0
        _Contrast("Contrast", Range(-2 , 4)) = 1
        _Sharpness("Sharpness", Range(0 , 1)) = 0
        _Temperature("Temperature", Range(-1, 1)) = 0
        _Tint1("Tint1", Range(-1, 1)) = 0
        _Tint2("Tint2", Range(-1, 1)) = 0
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

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


            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float _Brightness;
            float _Contrast;
            float _Sharpness;
            float _Temperature;
            float _Tint1;
            float _Tint2;


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

            float3 temperatureAdjustment(float3 color, float temperature)
            {
                float tintColor = temperature / 100.0; // Adjust this scaling factor as needed
                float redShift = saturate(1.0 - tintColor);
                float blueShift = saturate(tintColor);

                float3 adjustedColor = float3(
                    color.r * redShift + color.b * blueShift,
                    color.g * (1.0 - abs(tintColor * 0.5)),
                    color.b * redShift + color.r * blueShift
                    );

                return adjustedColor;
            }
            float3 tintAdjustment(float3 color, float2 tint)
            {
                float3 tintRGB = float3(1.0, tint.x, tint.y); // Tint color in RGB
                return color * tintRGB;
            }



            float3 sharpenPixel(v2f i)
            {
                float kernel[9] = { -1, -1, -1, -1, 9, -1, -1, -1, -1 };

                //float kernel[25] = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 25, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1};

                // Offset harcoded for now
                // Need to figure out how to get _MainTex_TexelSize
                float2 tex_offset = 1.0 / 512.0; 

                //float2 tex_offset = 1.0 / float2(_MainTex_TexelSize.xy);
                //float2 tex_offset = 1.0 / float2(_ScreenParams.xy);

                float3 original = tex2D(_MainTex, i.uv).rgb;

                //float3 pixel = original * kernel[12];
                float3 pixel = original * kernel[4];
                //pixel *=  kernel[4];


                //for (int x = -2; x <= 2; x++)
                //{
                //    for (int y = -2; y <= 2; y++)
                //    {
                //        if (x == 0 && y == 0) continue;

                //        float2 offset = float2(tex_offset.x * x, tex_offset.y * y);
                //        pixel += tex2D(_MainTex, i.uv + offset).rgb * kernel[(y + 2) * 5 + (x + 2)];
                //    }
                //}

                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if (x == 0 && y == 0) continue;

                        float2 offset = float2(tex_offset.x * x, tex_offset.y * y);
                        pixel += tex2D(_MainTex, i.uv + offset).rgb * kernel[(y + 1) * 3 + (x + 1)];
                    }
                }
                return lerp(original, pixel, _Sharpness);
            }

            float4 frag(v2f i) : SV_Target
            {


                // Get color of pixel
                //float3 color = tex2D(_MainTex, i.uv).rgb;w

                // Only sharpen if slider is greater than 8.0
                // Do we only want to increase strength of kernel or kernel size



                float3 color = sharpenPixel(i);

                color += _Brightness;
                color = (color - 0.5) * _Contrast + 0.5;


                //float3 color = tex2D(_MainTex, i.uv).rgb;
                //float color = _MainTex_TexelSize.y;

                //color = temperatureAdjustment(color, _Temperature);
                //color = tintAdjustment(color, float2(_Tint1, _Tint2));

                
                return float4(color, 1.0);
             
                
            }
            ENDCG
        }
    }
}