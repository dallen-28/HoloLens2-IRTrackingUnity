Shader "Custom/YUVtoRGBHololens"
{
    Properties
    {
        _YTex ("Y Texture", 2D) = "white" {}
        _UVTex ("UV Texture", 2D) = "black" {}
        _Sharpness("Sharpness", Range(0 , 1)) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // Enable instancing support for stereo rendering
            #pragma multi_compile_instancing

            sampler2D _YTex;
            sampler2D _UVTex;
            float _Sharpness;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); // Stereo rendering adjustment
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }


            // Converts YUV pixel value to RGB
            float3 yuvToRGB(float y, float2 uv, v2f i)
            {
                // Uncomment to incorporate vertical flipping
                //float2 flippedUV = float2(i.uv.x, 1.0 - i.uv.y);
                //float2 flippedUV = float2(i.uv.x, i.uv.y);

                //float y = tex2D(_YTex, flippedUV).a;
                //float2 uv = tex2D(_UVTex, flippedUV).rg - 0.5;
                float3 rgb = float3(y + 1.403 * uv.y,
                    y - 0.344 * uv.x - 0.714 * uv.y,
                    y + 1.770 * uv.x);

                return rgb;
            }


            float3 sharpenPixel(float3 rgb, v2f i)
            {
                float kernel[9] = { -1, -1, -1, -1, 9, -1, -1, -1, -1 };

                //float kernel[25] = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 25, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1};

                // Offset harcoded for now
                // Need to figure out how to get _MainTex_TexelSize
                float2 tex_offset = 1.0 / 512.0;

                //float2 tex_offset = 1.0 / float2(_MainTex_TexelSize.xy);
                //float2 tex_offset = 1.0 / float2(_ScreenParams.xy);
                float3 original = rgb;
                float3 pixel = original * kernel[4];

                //float2 flippedUV = float2(i.uv.x, 1.0 - i.uv.y);

                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if (x == 0 && y == 0) continue;
                        
                        // Calculate pixel offset


                        float2 offset = float2(tex_offset.x * x, tex_offset.y * y);
                        float alphaPixel = tex2D(_YTex, i.uv + offset).a;
                        float3 rgbPixel = (alphaPixel, alphaPixel, alphaPixel);
                        //pixel += tex2D(_MainTex, i.uv + offset).rgb * kernel[(y + 1) * 3 + (x + 1)];
                        pixel += rgbPixel * kernel[(y + 1) * 3 + (x + 1)];

                        //float2 offset = float2(tex_offset.x * x, tex_offset.y * y);
                        //float y = tex2D(_YTex, i.uv + offset).a;
                        //float2 uv = tex2D(_UVTex, i.uv + offset).rg - 0.5;


                        //float3 convertedPixel = yuvToRGB(y, uv, i);

                        // need to convert to YUV first
                        //pixel += convertedPixel * kernel[(y + 1) * 3 + (x + 1)];
                    }
                }
                return lerp(original, pixel, _Sharpness);
                //return pixel;

            }



            float4 frag (v2f i) : SV_Target
            {
                // Flip the texture vertically
                //float2 flippedUV = float2(i.uv.x, 1.0 - i.uv.y);
                float2 flippedUV = float2(i.uv.x, i.uv.y);

                float y = tex2D(_YTex, flippedUV).a;
                float2 uv = tex2D(_UVTex, flippedUV).rg - 0.5;

                float3 rgb = float3(y + 1.403 * uv.y,
                                    y - 0.344 * uv.x - 0.714 * uv.y,
                                    y + 1.770 * uv.x);


                //float pix = tex2D(_YTex, i.uv).a;
                //float3 rgb = (pix, pix, pix);

                //rgb = sharpenPixel(rgb, i);
                //float3 rgb = sharpenPixel(i);


                //float3 rgb = tex2D(_OutputTex, i.uv).rgb;

                return float4(rgb, 1.0);
            }
            ENDCG
        }
    }
}
