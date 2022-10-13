Shader "Chamoji/Murasaki/Wings"
{
	Properties
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Effect ("Effect", 2D) = "black" {}
		_Tint ("Effect Tint", Color) = (1, 1, 1, 1)
		_ScrollX ("X Scroll Speed", Float) = 0.5
		_ScrollY ("Y Scroll Speed", Float) = 0.0
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

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
				float2 texcoord : TEXCOORD1;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 texcoord : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			fixed4 _MainTex_ST;
			sampler2D _Effect;
			fixed4 _Tint;
			fixed _ScrollX;
			fixed _ScrollY;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

				o.texcoord.xy = v.texcoord.xy + frac(_Time.y * float2(_ScrollX, _ScrollY));
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				col.rgb += saturate((tex2D(_Effect, i.texcoord)) * _Tint);
				col.a += ((tex2D(_Effect, i.texcoord).r) * tex2D(_MainTex, i.uv).a);
				// apply fog
				return col;
			}
			ENDCG
		}
	}
}
