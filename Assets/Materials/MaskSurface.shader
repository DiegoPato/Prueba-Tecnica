Shader "Unlit/MaskSurface"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" { }
    }

    SubShader
    {
        Tags { "Queue" = "Geometry-1" }
        Stencil
        {
            Ref 1
            Comp NotEqual
        }

        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert 

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            // Sample the main texture
            fixed3 c = tex2D(_MainTex, IN.uv_MainTex).rgb;

            // Output color
            o.Albedo = c;
        }
        ENDCG
    }

    FallBack "Diffuse"
}
