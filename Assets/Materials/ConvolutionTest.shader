Shader "Convolution"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Sharpness("Sharpness", Range(0 , 30)) = 0
    }
        SubShader
        {
            // No culling or depth
            //Cull Off ZWrite Off ZTest Always

            Pass
            {
                CGPROGRAM

                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                float _Sharpness;

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            float3x3 GetData(int channel, sampler2D tex, float2 uv, float4 size)
            {
                float3x3 mat;
                for (int y = -1; y < 2; y++)
                {
                    for (int x = -1; x < 2; x++)
                    {
                        mat[x + 1][y + 1] = tex2D(tex, uv + float2(x * size.x, y * size.y))[channel];
                    }
                }
                return mat;
            }
            float3x3 GetMean(float3x3 matr, float3x3 matg, float3x3 matb)
            {
                float3x3 mat;
                for (int y = 0; y < 3; y++)
                {
                    for (int x = 0; x < 3; x++)
                    {
                        mat[x][y] = (matr[x][y] + matg[x][y] + matb[x][y]) / 3.0;
                    }
                }
                return mat;
            }

            float Convolve(float3x3 kernel, float3x3 pixels, float denom, float offset)
            {
                float res = 0.0;
                for (int y = 0; y < 3; y++)
                {
                    for (int x = 0; x < 3; x++)
                    {
                        res += kernel[2 - x][2 - y] * pixels[x][y];
                    }
                }

                return  res;
            }

            //fixed4 frag(v2f i) : SV_Target
            //{
            //    float3x3 kerIdentity = float3x3 (-1.0,  -1.0,  -1.0,
            //                                        -1.0,  8.0,  -1.0,
            //                                        -1.0,  -1.0,  -1.0);

            //   float3x3 kerEmboss = float3x3 (2.0,  0.0,  0.0,
            //                                 0.0, -1.0,  0.0,
            //                                 0.0,  0.0, -1.0);

            //   float3x3 kerSharpness = float3x3 (0, -1.0, 0,
            //                                    -1.0,  _Sharpness, -1.0,
            //                                    0, -1.0, 0);

            //   float3x3 kerGausBlur = float3x3 (1.0, 2.0, 1.0,
            //                                    2.0, 4.0, 2.0,
            //                                    1.0, 2.0, 1.0);

            //    float3x3 kerEdgeDetect = float3x3 (-1.0,  -1.0,  -1.0,
            //                                        -1.0,  8.0,  -1.0,
            //                                        -1.0,  -1.0,  -1.0);

            //    float3x3 matr = GetData(0, _MainTex, i.uv, _MainTex_TexelSize);
            //    float3x3 matg = GetData(1, _MainTex, i.uv, _MainTex_TexelSize);
            //    float3x3 matb = GetData(2, _MainTex, i.uv, _MainTex_TexelSize);
            //    float3x3 mata = GetMean(matr, matg, matb);


            //    // kernel
            //   float4 gl_FragColor = float4(Convolve(kerSharpness,matr,1.0,0.0),
            //                                Convolve(kerSharpness,matg,1.0,0.0),
            //                                Convolve(kerSharpness,matb,1.0,0.0),
            //                                1.0);

            //    return gl_FragColor;
            //}

            fixed4 frag(v2f i) : SV_Target
            {
                float3x3 kerSharpness = float3x3(0, -1.0, 0,
                                                  -1.0, 5.0 + _Sharpness, -1.0,
                                                  0, -1.0, 0);

                float3x3 matr = GetData(0, _MainTex, i.uv, _MainTex_TexelSize);
                float3x3 matg = GetData(1, _MainTex, i.uv, _MainTex_TexelSize);
                float3x3 matb = GetData(2, _MainTex, i.uv, _MainTex_TexelSize);
                float3x3 mata = GetMean(matr, matg, matb);

                // Apply the convolution to each channel separately
                float3 convResult = float3(Convolve(kerSharpness, matr, 1.0, 0.0),
                                            Convolve(kerSharpness, matg, 1.0, 0.0),
                                            Convolve(kerSharpness, matb, 1.0, 0.0));

                // Combine the result and set alpha to 1.0
                float4 result = float4(convResult, 1.0);

                return result;
            }


            ENDCG
        }
    }
}