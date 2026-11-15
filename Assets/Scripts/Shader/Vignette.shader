Shader "Custom/Vignette"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        _VignetteRadius("Radius",Float) = 0.5
        _VignetteFeather("Feather",Float) = 0.5
        _VignetteColour("Colour",Color) = (0,1,0,1)




        
    }

    SubShader
    {
         Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
        }       
        ZWrite Off
        Ztest Always
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
        
        HLSLINCLUDE
        
         #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
         #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

         
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

        Pass 
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
                    
            float _VignetteRadius,_VignetteFeather;
            float4 _VignetteColour;
           
            
            v2f vert(MeshData v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _BaseMap);
                return o;
            }
                    
            float4 frag (v2f i) : SV_Target
            {
                float4 baseCol = 
                SAMPLE_TEXTURE2D_X_LOD(_BlitTexture,sampler_LinearClamp, i.uv,_BlitMipLevel);
                
                return SAMPLE_TEXTURE2D_X_LOD(_BlitTexture,sampler_LinearClamp, i.uv,_BlitMipLevel);
                
               // return float4(i.uv,0,1);

                //Apply vignette
                float2 centerUVS = (float2 (i.uv * 2 - 1));
                float4 vignetteMask = baseCol * (smoothstep(_VignetteRadius,_VignetteRadius+_VignetteFeather,length(centerUVS)));
                float4 finalCol = vignetteMask * _VignetteColour;
            
                return finalCol;
                
           
               
            }
            ENDHLSL
        }

       

        
    }
}
