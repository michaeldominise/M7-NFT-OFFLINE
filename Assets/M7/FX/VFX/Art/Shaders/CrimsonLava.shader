Shader "Chamoji/Murasaki/CrimsonLava"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Tint ("Color", color) = (1, 1, 1, 1)
		_Effect ("Effect", color) = (1, 1, 1, 1)
		_ScrollX ("X Scroll Speed", float) = 0.0
		_ScrollY ("Y Scroll Speed", float) = 0.1
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
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			fixed4 _MainTex_ST;
			fixed4 _Effect;
			fixed4 _Tint;
			fixed _ScrollX;
			fixed _ScrollY;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				o.uv.xy = v.uv.xy + frac(_Time.y * float2(_ScrollX, _ScrollY));
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col;
				col.rgb = lerp(_Tint, _Effect, tex2D(_MainTex, i.uv).bbb);
				
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
