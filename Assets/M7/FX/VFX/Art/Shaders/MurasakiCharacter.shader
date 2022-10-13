Shader "Chamoji/Murasaki/Character_v2"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ShadeDefault ("Default Shade", Color) = (0.6784, 0.6784, 0.6784, 1)
		_Shade ("Shade Color", Color) = (0.9058, 0.6431, 0.6431, 1)
		_Mask ("Masks (RGB Channels)", 2D) = "black" {}
		_RimColor ("Rim Color", Color) = (1, 1, 1, 1)
		_RimPower ("Rim Power", Range(0.5, 8.0)) = 7.0
		_SpecColor ("Specular Color",Color) = (1, 1, 1, 1)
		_SpecStr ("Specular Strength", Range(0.0, 8.0)) = 0.0
		_Outline ("Outline", Range(0, 20)) = 0
		_OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
		[NoScaleOffset] _EmitMap ("Emission Map", 2D) = "black" {}
		_EmitColor ("Emission Color", Color) = (1, 1, 1, 1)
		_EmitPower ("Emission Intensity", Range(0,1)) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			Tags { "LightMode"="ForwardBase" }
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			
			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"
			#include "AutoLight.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float3 lightDir: TEXCOORD1;
				float3 viewDir : NORMAL1;
				SHADOW_COORDS(2)
			};

			sampler2D _MainTex;
			fixed4 _MainTex_ST;
			sampler2D _Mask;
			sampler2D _EmitMap;

			fixed4 _Shade;
			fixed4 _ShadeDefault;
			fixed4 _RimColor;
			fixed _RimPower;
			fixed _SpecStr;
			fixed4 _EmitColor;
			fixed _EmitPower;
			
			v2f vert (appdata v)
			{
				v2f o = (v2f)0;
				
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				o.normal = UnityObjectToWorldNormal(v.normal);
				o.lightDir = _WorldSpaceLightPos0.xyz;
				o.viewDir = WorldSpaceViewDir(v.vertex);

				//TRANSFER_SHADOW(o)

				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				i.normal = normalize(i.normal);
				i.lightDir = normalize(i.lightDir);
				i.viewDir = normalize(i.viewDir);

				half nl = max(0.0, (dot(i.normal, i.lightDir) * 0.5 + 0.5));
				half rim = 1.0 - saturate(dot (i.viewDir, i.normal));

				half3 halfDir = normalize(i.viewDir + i.lightDir);
				
				half nh = max(0.0, dot(i.normal, halfDir));
				half nv = max(0.0, dot(i.normal, i.viewDir));

				half nlsqr = nl * nl;

				fixed shadow = saturate(SHADOW_ATTENUATION(i));
				fixed shadowLight = shadow * nl;
				fixed3 shadedef = lerp(_ShadeDefault, 1, shadowLight);
				fixed3 shade = lerp(_Shade, 1, shadowLight);
				fixed3 finalshade = lerp(shadedef, shade, (tex2D(_Mask, i.uv).rrr));
				fixed3 toon = lerp(1, finalshade, (1 - tex2D(_Mask, i.uv).ggg));
				fixed3 rimLight = _RimColor * pow(rim, _RimPower);
				fixed3 spec = pow(nl, _SpecStr * 16) * tex2D(_Mask, i.uv).bbb * _SpecColor;
				fixed3 emit = pow(_EmitPower, _EmitColor.a) * tex2D(_EmitMap, i.uv).rgb * _EmitColor;

				fixed4 col = tex2D(_MainTex, i.uv);
				col.rgb *= toon;
				col.rgb += rimLight + spec + emit;
				//col.rgb += spec;
				
				return col;
			}
			ENDCG
		}

		Pass {
			Cull Front

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_fog
			#pragma multi_compile_instancing

			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
			};

			float _Outline;
			fixed4 _OutlineColor;

			v2f vert(appdata v) {
				v2f o = (v2f)0;

				o.vertex = UnityObjectToClipPos(v.vertex);

				o.vertex += _Outline * 0.0001;

				return o;
			}

			fixed4 frag(v2f i) : SV_Target {
				fixed4 col = _OutlineColor;
				return col;
			}
			
			ENDCG
		}

		Pass {	
			Tags {"LightMode" = "ShadowCaster"}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#include "UnityCG.cginc"

			struct v2f {
				V2F_SHADOW_CASTER;
			};

			v2f vert(appdata_base v) {
				v2f o = (v2f)0;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
				return o;
			}

			fixed4 frag(v2f i) : SV_Target {
				SHADOW_CASTER_FRAGMENT(i)
			}

			ENDCG
		}
	}
}
