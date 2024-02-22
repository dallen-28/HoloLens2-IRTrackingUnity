Shader "Custom/TwoPassShader" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
    }

        SubShader{
        // First Pass: Convert to Grayscale
        Pass {
            Tags { "LightMode" = "ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_gray
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            half4 frag_gray(v2f i) : COLOR {
                half4 col = tex2D(_MainTex, i.uv);
                half gray = dot(col.rgb, float3(0.299, 0.587, 0.114));
                return half4(gray, gray, gray, col.a);
            }

            ENDCG
        }

        // Second Pass: Apply Sepia Tone
        Pass {
            Tags { "LightMode" = "ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_sepia
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            half4 frag_sepia(v2f i) : COLOR {
                half4 col = tex2D(_MainTex, i.uv);
                half3 sepia = half3(dot(col.rgb, half3(0.393, 0.769, 0.189)),
                                    dot(col.rgb, half3(0.349, 0.686, 0.168)),
                                    dot(col.rgb, half3(0.272, 0.534, 0.131)));
                return half4(sepia, col.a);
            }

            ENDCG
        }
    }
        FallBack "Diffuse"
}
