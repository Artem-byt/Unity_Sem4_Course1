Shader "Star"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" } 
        Blend 
        SrcAlpha 
        OneMinusSrcAlpha 
        Cull Off 
        Lighting Off 
        ZWrite Off

        Pass 
        { 
            CGPROGRAM 
            #pragma vertex vert 
            #pragma fragment frag 
            struct data 
        { 
            float4 vertex : POSITION; 
            fixed4 color : COLOR; 
        };
        
        data vert(data v) 
        {
            v.vertex = UnityObjectToClipPos(v.vertex);
            return v; 
        } 
        
        fixed4 frag(data f) : COLOR 
        { 
            return f.color; 
        } 
            ENDCG 
        } 
    }
}
