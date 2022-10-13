Shader "Chamoji/Murasaki/ArenaDiffuse" {
        Properties {
            _MainTex ("Texture", 2D) = "white" {}
			_Tint ("Tint", color) = (0, 0, 0, 0)
        }
        SubShader {
        Tags { "RenderType" = "Opaque" }
        CGPROGRAM
          #pragma surface surf SimpleLambert

		  float _this;
		  float _that;
  
          half4 LightingSimpleLambert (SurfaceOutput s, half3 lightDir, half atten) {
              half NdotL = dot (s.Normal, lightDir) * 0.4 + 0.4;
              half4 c;
              c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten);
              c.a = s.Alpha;
              return c;
          }
  
        struct Input {
            float2 uv_MainTex;
        };
        
        sampler2D _MainTex;
		fixed4 _Tint;
        
        void surf (Input IN, inout SurfaceOutput o) {
            o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
			o.Emission = tex2D (_MainTex, IN.uv_MainTex).a * _Tint;
        }
        ENDCG
        }
        Fallback "Diffuse"
    }