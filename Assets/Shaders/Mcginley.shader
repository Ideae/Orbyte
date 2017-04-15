// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/Mcginley" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_SecondaryColor ("SecondaryColor", Color) = (0,0,0,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		[MaterialToggle] _Gravity ("Gravity", Float) = 0
		_Frequency ("Frequency", Float) = 1.0
		_Wavelength ("Wavelength", Float) = 20
		_Threshold ("Threshold", Float) = 0.7
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed4 _SecondaryColor;
		float _Gravity;
		float _Frequency;
		float _Wavelength;
		float _Threshold;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		fixed3 GravityFunc(fixed3 coord) {
			float dist = sqrt(pow(coord.x,2) + pow(coord.y,2));
			float wave = sin(_Time.w * _Frequency + dist * _Wavelength) * 0.5f + 0.5f;
			wave = floor(wave + _Threshold);
		  if (wave < 0.5) {
				return _Color;
			}
			else {
				return _SecondaryColor;
			}
			//return fixed3(wave, wave, wave);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			//float4 f = float4(IN.worldPos.x,IN.worldPos.y,IN.worldPos.z,1);
			o.Albedo = c;
			fixed3 objpos = mul (unity_WorldToObject, float4(IN.worldPos, 1));

			fixed3 coord = objpos - fixed3(0,0,0);
			if (_Gravity > 0) {
				fixed3 grav = GravityFunc(coord);
				o.Albedo = grav;
			}
			//o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			//o.Metallic = _Metallic;
			//o.Smoothness = _Glossiness;
			//o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
