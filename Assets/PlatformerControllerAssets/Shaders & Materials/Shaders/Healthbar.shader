Shader "Unlit/Healthbar"{
    Properties{
        [NoScaleOffset] _MainTex("Texture", 2D) = "white" {}
        _Health("Health", Range(0,1)) = 1
    }
        SubShader{
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent"}

            Pass {
                ZWrite On // Renders above tilemap
                Cull Off
                Blend SrcAlpha OneMinusSrcAlpha // Alpha Blending

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct MeshData {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct Interpolators {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float _Health;

                Interpolators vert(MeshData v) {
                    Interpolators o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                float InverseLerp(float a, float b, float v) {
                    return (v - a) / (b - a);
                }

                float4 frag(Interpolators i) : SV_Target{

                    float healthBarMask = _Health > i.uv.x; // Create Mask with slidable range

                    float flash = cos(_Time.y * 4) * 0.3 + 1;
                    float3 healthBarColor = tex2D(_MainTex, float2(_Health, i.uv.y));

                    /*if(_Health < 0.2)
                        healthBarColor *= flash;*/

                    return float4(healthBarColor * healthBarMask, healthBarMask);

                }
                ENDCG
            }
        }
}
