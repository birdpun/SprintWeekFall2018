Shader "Custom/Cel Shader"
{
	Properties
	{
		_Color ("Colour", Color) = (1,1,1,1)
		_Cutoff ("Alphatest Cutoff", Range(0, 1)) = 0.5
		_MainTex ("Diffuse (RGB) Alpha (A)", 2D) = "white" {}
		_RampTex ("Ramp (RGB)", 2D) = "white" {}
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		
		CGPROGRAM

		#pragma surface surf TF2 alphatest:_Cutoff
		#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
			float3 worldNormal;
			INTERNAL_DATA
		};

		sampler2D _MainTex, _RampTex;
		float4 _Color;

		inline fixed4 LightingTF2 (SurfaceOutput s, fixed3 lightDir, fixed3 viewDir, fixed atten)
		{
			fixed NdotL = dot(normalize(s.Normal), normalize(lightDir)) * 0.5 + 0.5;
			fixed3 ramp = tex2D(_RampTex, float2(saturate(NdotL * atten), 1.0)).rgb;
				
			fixed4 c;
			c.rgb = (s.Albedo * _Color) * ramp;
			c.a = s.Alpha;

			return c;
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
			o.Alpha = tex2D(_MainTex, IN.uv_MainTex).a;
		}

		ENDCG
	}

	Fallback "Transparent/Cutout/Bumped Specular"
}