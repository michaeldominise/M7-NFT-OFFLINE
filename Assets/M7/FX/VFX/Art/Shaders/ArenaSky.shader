    // Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)
     
    Shader "Chamoji/Murasaki/ArenaSky" {
        Properties{
     
     
            _Rotation("Rotation", float) = 0
            [NoScaleOffset] _FrontTex("Front [+Z]", 2D) = "grey" {}
        	[NoScaleOffset] _BackTex("Back [-Z]", 2D) = "grey" {}
	        [NoScaleOffset] _LeftTex("Left [+X]", 2D) = "grey" {}
        	[NoScaleOffset] _RightTex("Right [-X]", 2D) = "grey" {}
        	[NoScaleOffset] _UpTex("Up [+Y]", 2D) = "grey" {}
	        [NoScaleOffset] _DownTex("Down [-Y]", 2D) = "grey" {}
        }
     
        SubShader{
        Tags{ "Queue" = "Background" "RenderType" = "Background" "PreviewType" = "Skybox" }
        Cull Off ZWrite Off
     
        CGINCLUDE
    	#include "UnityCG.cginc"
     
     
     
        float _Rotation;
     
        float3 RotateAroundYInDegrees(float3 vertex, float degrees)
        {
            float alpha = degrees * UNITY_PI / 180.0;
            float sina, cosa;
            sincos(alpha, sina, cosa);
            float2x2 m = float2x2(cosa, -sina, sina, cosa);
            return float3(vertex.x,mul(m, vertex.yz)).zxy;
        }
     
        struct appdata_t {
            float4 vertex : POSITION;
            float2 texcoord : TEXCOORD0;
       
        };
        struct v2f {
            float4 vertex : SV_POSITION;
            float2 texcoord : TEXCOORD0;
       
        };
        v2f vert(appdata_t v)
        {
            v2f o;  
            float3 rotated = RotateAroundYInDegrees(v.vertex, _Rotation);
            o.vertex = UnityObjectToClipPos(rotated);
            o.texcoord = v.texcoord;
            return o;
        }
        float4 skybox_frag(v2f i, sampler2D smp, float4 smpDecode)
        {
            float4 tex = tex2D(smp, i.texcoord);
            return tex;
        }
        ENDCG
     
            Pass{
            CGPROGRAM
    #pragma vertex vert
    #pragma fragment frag
    #pragma target 2.0
            sampler2D _FrontTex;
        float4 frag(v2f i) : SV_Target{return  tex2D(_FrontTex, i.texcoord);
        }
            ENDCG
        }
            Pass{
            CGPROGRAM
    #pragma vertex vert
    #pragma fragment frag
    #pragma target 2.0
            sampler2D _BackTex;
        float4 frag(v2f i) : SV_Target{ return  tex2D(_BackTex, i.texcoord); }
            ENDCG
        }
            Pass{
            CGPROGRAM
    #pragma vertex vert
    #pragma fragment frag
    #pragma target 2.0
            sampler2D _LeftTex;
        float4 frag(v2f i) : SV_Target{ return  tex2D(_LeftTex, i.texcoord); }
            ENDCG
        }
            Pass{
            CGPROGRAM
    #pragma vertex vert
    #pragma fragment frag
    #pragma target 2.0
            sampler2D _RightTex;
        float4 frag(v2f i) : SV_Target{ return  tex2D(_RightTex, i.texcoord); }
            ENDCG
        }
            Pass{
            CGPROGRAM
    #pragma vertex vert
    #pragma fragment frag
    #pragma target 2.0
            sampler2D _UpTex;
        float4 frag(v2f i) : SV_Target{ return  tex2D(_UpTex, i.texcoord); }
            ENDCG
        }
            Pass{
            CGPROGRAM
    #pragma vertex vert
    #pragma fragment frag
    #pragma target 2.0
            sampler2D _DownTex;
        float4 frag(v2f i) : SV_Target{ return  tex2D(_DownTex, i.texcoord); }
            ENDCG
        }
        }
    }
     
