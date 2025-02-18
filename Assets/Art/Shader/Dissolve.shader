﻿Shader "Unlit/Dissolve"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_DissolveTex ("Dissolve Texture", 2D) = "white" {}
		_Dissolve ("Dissolve Strength", Float) = 0
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

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			sampler2D _DissolveTex;
			float _Dissolve;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

			fixed4 frag(v2f i) : SV_Target
			{
				float transition = _Dissolve - i.worldPos.y;
				clip(i.worldPos.y + transition + tex2D(_DissolveTex, i.uv));

                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
            

                return col;
            }
            ENDCG
        }
    }
}
