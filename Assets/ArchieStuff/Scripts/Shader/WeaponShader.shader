Shader "Custom/WeaponShader"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        _saturationAlpha("Saturation alpha", Float) = 1
        _dissolveAlphaThreshold("Dissolve alpha threshold",Float) = 1

        
    }

    SubShader
    {
        Tags { "RenderType" = "TransparentCutout" "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent"}
        ZWrite On
        Blend SrcAlpha OneMinusSrcAlpha
        
        HLSLINCLUDE
        
         #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
         #include "noise.cginc"
         
         struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
         struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;
            CBUFFER_END
            
            
        
        ENDHLSL

        Pass //Desaturate colour
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
                    
            float _saturationAlpha;
            float _dissolveAlphaThreshold;
            
            v2f vert(MeshData v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _BaseMap);
                return o;
            }
                    
            float4 frag (v2f i) : SV_Target
            {
                float4 baseCol = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv) * _BaseColor;
                float3 desaturateCol = dot(baseCol.xyz,float3(0.212,0.715,0.072));
                float4 finalCol = float4(lerp(desaturateCol,baseCol.xyz, _saturationAlpha).xyz,1.0f);
                
                float noiseAlpha;
                Unity_SimpleNoise_float(i.uv,50, noiseAlpha);
                
                
                if (noiseAlpha < _dissolveAlphaThreshold){return finalCol;}
                return float4(0,0,0,0);
                
               
            }
            ENDHLSL
        }

       

        
    }
}
