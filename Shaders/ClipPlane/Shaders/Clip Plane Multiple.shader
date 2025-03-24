Shader "Clip Plane/InverseCube"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Main Texture", 2D) = "white" {}

        [MaterialToggle] _UseWorldSpace("Use World Space", Float) = 0
        _PlaneVector1("Plane Vector 1", Vector) = (-1, 0, 0, 1)
        _PlaneVector2("Plane Vector 2", Vector) = (1, 0, 0, 1)
        _PlaneVector3("Plane Vector 3", Vector) = (0, -1, 0, 1)
        _PlaneVector4("Plane Vector 4", Vector) = (0, 1, 0, 1)
        _PlaneVector5("Plane Vector 5", Vector) = (0, 0, -1, 1)
        _PlaneVector6("Plane Vector 6", Vector) = (0, 0, 1, 1)
    }

    SubShader
    {
        Tags{ "RenderType" = "Opaque" "Queue" = "Geometry" }

        Pass
        {
            Cull Off

            Stencil
            {
                Comp Always
                PassFront IncrWrap
                FailFront IncrWrap

                PassBack DecrWrap
                FailBack DecrWrap

                ZFailFront IncrWrap
                ZFailBack DecrWrap
            }

            CGPROGRAM
            #pragma target 3.0
            #include "UnityCG.cginc"
            #include "./Clip Plane Functions.cginc"
            #pragma vertex vert
            #pragma fragment frag

            uniform fixed4 _LightColor0;

            float4 _Color;
            float4 _MainTex_ST;         // For the Main Tex UV transform
            sampler2D _MainTex;         // Texture used for the line
            float4 _PlaneVector1;
            float4 _PlaneVector2;
            float4 _PlaneVector3;
            float4 _PlaneVector4;
            float4 _PlaneVector5;
            float4 _PlaneVector6;
            float _UseWorldSpace;

            struct v2f
            {
                float4 pos      : POSITION;
                float4 col      : COLOR;
                float2 uv       : TEXCOORD0;
                float doclip    : TEXCOORD1;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

                // Calculate clip value for multiple planes
                float3 wp = mul(unity_ObjectToWorld, v.vertex).xyz;
                float clip1 = isClipped(_PlaneVector1, wp, _UseWorldSpace);
                float clip2 = isClipped(_PlaneVector2, wp, _UseWorldSpace);
                float clip3 = isClipped(_PlaneVector3, wp, _UseWorldSpace);
                float clip4 = isClipped(_PlaneVector4, wp, _UseWorldSpace);
                float clip5 = isClipped(_PlaneVector5, wp, _UseWorldSpace);
                float clip6 = isClipped(_PlaneVector6, wp, _UseWorldSpace);
                // Combine clip values (e.g., using min to clip if all planes clip)
                o.doclip = min(min(min(clip1, clip2), min(clip3, clip4)), min(clip5, clip6));
                
                // Lighting 
                float4 norm = mul(unity_ObjectToWorld, v.normal);
                float3 normalDirection = normalize(norm.xyz);
                float4 AmbientLight = UNITY_LIGHTMODEL_AMBIENT;
                float4 LightDirection = normalize(_WorldSpaceLightPos0);
                float4 DiffuseLight = saturate(dot(LightDirection, normalDirection))*_LightColor0;
                o.col = float4(AmbientLight + DiffuseLight);

                return o;
            }

            float4 frag(v2f i) : COLOR
            {
                clip(i.doclip);

                float4 col = _Color * tex2D(_MainTex, i.uv);
                return col * i.col;
            }

            ENDCG
        }
    }
}