Shader "Unlit/Shader1"
{
    Properties
    {
        // _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _Color;

            struct MeshData{
                float4 vertex : POSITION;
                float3 normals : NORMAL;
                float4 uv0 : TEXCOORD0;
            };

            struct Interpolators{
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD0;
                // float2 uv : TEXCOORD0;
            };

            Interpolators vert (MeshData v){
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = v.normals; //UnityObjectToWorldNormal (v.normals);
                return o;
            }

            float4 frag (Interpolators i) : SV_Target{

                return float4( UnityObjectToWorldNormal(i.normal), 1);
            }
            ENDCG
        }
    }
}
