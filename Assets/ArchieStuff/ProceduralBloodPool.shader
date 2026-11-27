Shader "Custom/ProceduralBloodPool"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        _Size("Size",Float) = 0.5
        _Seed("Seed", Float) = 1
        _bigNumber("Big number",Int) = 1
    }



    SubShader
    {
        Tags { "RenderType" = "TransparentCutout" "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent"}

        ZWrite On
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            
            #pragma vertex vert 
            #pragma fragment frag 
            
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

            uint NextRandom(inout uint state)
            {
                state = state * 747796405 + 2891336453;
                uint result = ((state >> ((state >> 28) + 4)) ^ state) * 277803737;
                result = (result >> 22) ^ result;
                return result;
            }

            float RandomValue(inout uint state)
            {
                return NextRandom(state) / 4294967295.0; // 2^32 - 1
            }
            
            
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;
            CBUFFER_END
            
            float _Size;
            int _Seed;
            uint _bigNumber;
            

            v2f vert(MeshData v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _BaseMap);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
               float2 centerUVs = (i.uv * 2 ) - 1;
               
               float angle = atan2(centerUVs.y,centerUVs.x);
               
  
               
               float distortion;
               //More blob, less splatter
              //  Unity_SimpleNoise_float(float2((cos(angle) * 2.75)+ RandomValue(_bigNumber),(sin(angle) * 0.85))+ RandomValue(_bigNumber) * _Seed * 0.3f,0.5 + RandomValue(_bigNumber),distortion);
               
               Unity_SimpleNoise_float(float2((cos(angle) * 1.75)+ RandomValue(_bigNumber),(sin(angle) * 5.85))+ RandomValue(_bigNumber) * _Seed * 0.6f,0.5 + RandomValue(_bigNumber),distortion);
               
               float radialDist = length(centerUVs);
               
                
               float4 mask = step(radialDist - distortion,_Size);
               
  
               return mask * float4(i.uv,0,1);
            }
            ENDHLSL
        }
    }
}
