Shader "Custom/VirtualDisplayShader" {
    Properties{

        _Brightness("Brightness", Range(-1, 1)) = 0
        _Contrast("Contrast", Range(-2 , 4)) = 1
        _Sharpness("Sharpness", Range(0, 2)) = 0
        _Temperature("Temperature", Range(-1, 1)) = 0
        _Tint("Tint", Range(-1, 1)) = 0
    }

        SubShader{
            Tags { "Queue" = "Overlay" }

            CGPROGRAM
            #pragma surface surf Lambert

            struct Input {
                float2 uv_MainTex;
            };

            sampler2D _YTex;  // Y texture
            sampler2D _UVTex;  // CbCr texture
            float _Brightness;
            float _Contrast;
            float _Sharpness;
            float _Temperature;
            float _Tint;

            // Sample NV12 texture and convert to RGB
            fixed4 SampleNV12Texture(float2 uv) : COLOR {
                float Y = tex2D(_YTex, uv).a;
                //float Cb = tex2D(_CbCrTex, uv).r - 0.5;
                //float Cr = tex2D(_CbCrTex, uv).g - 0.5;
                    
                float2 UV = tex2D(_UVTex, uv).rg - 0.5;

                //float Cb = tex2D(_CbCrTex, uv).r - 0.5;
                //float Cr = tex2D(_CbCrTex, uv).g - 0.5;

                // Convert YCbCr to RGB
                float3 color = float3(Y + 1.403 * UV.x, Y - 0.344 * UV.y - 0.714 * UV.x, Y + 1.770 * UV.y);
                


                //float3 color = float3(Y, Y, Y);

                // Set alpha to full transparency
                return fixed4(color, 1.0);
            }

                // Apply brightness, contrast, sharpness, temperature, and tint
            fixed4 ApplyAdjustments(fixed4 inputColor) {
                // Apply brightness
                inputColor.rgb += _Brightness;

                // Apply contrast
                inputColor.rgb = (inputColor.rgb - 0.5) * _Contrast + 0.5;

                // Apply sharpness
                inputColor.rgb = lerp(inputColor.rgb, inputColor.rgb + (inputColor.rgb - 0.5) * _Sharpness, _Sharpness);

                // Apply temperature and tint
                float3 colorTemp = inputColor.rgb;
                colorTemp = colorTemp * (1.0 + _Temperature);
                colorTemp.r = colorTemp.r + _Tint;
                colorTemp.b = colorTemp.b - _Tint;
                inputColor.rgb = colorTemp;

                // Set alpha to full transparency
                inputColor.a = 1.0;

                return inputColor;
            }

            void surf(Input IN, inout SurfaceOutput o) {
                // Sample the NV12 textures and convert to RGB
                fixed4 col = SampleNV12Texture(IN.uv_MainTex);

                // Apply adjustments
                col = ApplyAdjustments(col);

                // Output final color
                o.Albedo = col.rgb;
                o.Alpha = col.a;
            }
            ENDCG
        }

            Fallback "Diffuse"
}
