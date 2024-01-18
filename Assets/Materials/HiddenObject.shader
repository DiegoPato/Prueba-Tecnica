Shader "Unlit/HiddenObject"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" { }
        _Alpha ("Alpha", Range (0.0, 1.0)) = 1.0
    }

    SubShader
    {
        Tags { "Queue" = "Geometry-2" }
        Stencil
        {
            Ref 1
            Comp Always
            Pass Replace
        }

        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert alpha

        sampler2D _MainTex;
        fixed _Alpha;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            // Sample the main texture
            fixed3 c = tex2D(_MainTex, IN.uv_MainTex).rgb;

            // Output color with alpha
            o.Albedo = c;
            o.Alpha = _Alpha;
        }
        ENDCG
    }

    FallBack "Diffuse"
}