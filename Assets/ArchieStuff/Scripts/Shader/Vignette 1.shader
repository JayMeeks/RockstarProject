Shader "Custom/Vignette1"
{   
    SubShader
    {
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
        ENDHLSL

        Tags { "RenderType"="Opaque" }
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "Vignette1"

            HLSLPROGRAM
            
            #pragma vertex Vert
            #pragma fragment Frag
            
            float _VignetteRadius,_VignetteFeather;
            float3 _VignetteColour;

            float4 Frag (Varyings input) : SV_Target
            {
                float4 baseCol = 
                SAMPLE_TEXTURE2D_X_LOD(_BlitTexture,sampler_LinearClamp, input.texcoord,_BlitMipLevel);
                
               // return SAMPLE_TEXTURE2D_X_LOD(_BlitTexture,sampler_LinearClamp, input.texcoord,_BlitMipLevel);
                
               // return float4(i.uv,0,1);

                //Apply vignette
                float2 centerUVS = (float2 (input.texcoord * 2 - 1));
                float4 vignetteMask = baseCol * (1-smoothstep(_VignetteRadius,_VignetteRadius+_VignetteFeather,length(centerUVS)));
                float4 finalCol = vignetteMask;
            
                return finalCol;
            }
            
            ENDHLSL
        }
    }
}
