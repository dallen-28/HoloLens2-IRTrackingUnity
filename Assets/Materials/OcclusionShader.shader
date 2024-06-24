Shader "Custom/OcclusionShader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
    }

        SubShader
    {
        Tags { "RenderType" = "Opaque" }

        Pass
        {
            ZWrite On
            ZTest LEqual
            ColorMask 0
        }
    }
}