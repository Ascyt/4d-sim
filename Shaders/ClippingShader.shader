Shader "Custom/ClippingShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _MinBounds ("Min Bounds", Vector) = (-1, -1, -1)
        _MaxBounds ("Max Bounds", Vector) = (1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float3 _MinBounds;
            float3 _MaxBounds;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.color = v.color * _Color; // Combine vertex color with the property color
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // Clip the geometry based on the world position
                if (i.worldPos.x < _MinBounds.x || i.worldPos.x > _MaxBounds.x ||
                    i.worldPos.y < _MinBounds.y || i.worldPos.y > _MaxBounds.y ||
                    i.worldPos.z < _MinBounds.z || i.worldPos.z > _MaxBounds.z)
                {
                    discard;
                }

                half4 tex = tex2D(_MainTex, i.uv);
                return tex * i.color; // Modulate texture color with vertex color
            }
            ENDCG
        }
    }
}