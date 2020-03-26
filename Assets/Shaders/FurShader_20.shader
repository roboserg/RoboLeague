Shader "Custom/FurShader_20" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		_FurLength ("Fur Length", Range (.0002, 1)) = .25
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5 // how "thick"
		_CutoffEnd ("Alpha cutoff end", Range(0,1)) = 0.5 // how thick they are at the end
		_EdgeFade ("Edge Fade", Range(0,1)) = 0.4

		_Gravity ("Gravity direction", Vector) = (0,0,1,0)
		_GravityStrength ("G strenght", Range(0,1)) = 0.25

		_RimColor("Rim Color", Color) = (1,1,1,1)
		_RimPower("Rim Power", Range(0,20)) = 1
		_EmissiveColor("Emissive Color", Color) = (1,1,1,1)
		_EmissiveIntensity("Emissive Intensity", Range(0,20)) = 1
	}

	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		ZWrite On
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		fixed4 _Color;
		sampler2D _MainTex;
		half _Glossiness;
		half _Metallic;

		fixed4 _RimColor;
		fixed _RimPower;
		float4 _EmissiveColor;
		float _EmissiveIntensity;

		struct Input {
			float2 uv_MainTex;
			fixed3 viewDir;
		};

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;

			half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
			o.Emission = _RimColor.rgb * pow(rim, _RimPower);
		}
		ENDCG

		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
		#define FUR_MULTIPLIER 0.05
		#include "FurHelpers.cginc"
		ENDCG

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
		#define FUR_MULTIPLIER 0.1
		#include "FurHelpers.cginc"
		ENDCG

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
		#define FUR_MULTIPLIER 0.15
		#include "FurHelpers.cginc"
		ENDCG

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
		#define FUR_MULTIPLIER 0.20
		#include "FurHelpers.cginc"
		ENDCG

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
		#define FUR_MULTIPLIER 0.25
		#include "FurHelpers.cginc"
		ENDCG

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
		#define FUR_MULTIPLIER 0.30
		#include "FurHelpers.cginc"
		ENDCG

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
		#define FUR_MULTIPLIER 0.35
		#include "FurHelpers.cginc"
		ENDCG

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
		#define FUR_MULTIPLIER 0.40
		#include "FurHelpers.cginc"
		ENDCG

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
		#define FUR_MULTIPLIER 0.45
		#include "FurHelpers.cginc"
		ENDCG

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
		#define FUR_MULTIPLIER 0.50
		#include "FurHelpers.cginc"
		ENDCG

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
		#define FUR_MULTIPLIER 0.55
		#include "FurHelpers.cginc"
		ENDCG

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
		#define FUR_MULTIPLIER 0.60
		#include "FurHelpers.cginc"
		ENDCG

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
		#define FUR_MULTIPLIER 0.65
		#include "FurHelpers.cginc"
		ENDCG

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
		#define FUR_MULTIPLIER 0.70
		#include "FurHelpers.cginc"
		ENDCG

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
		#define FUR_MULTIPLIER 0.75
		#include "FurHelpers.cginc"
		ENDCG

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
		#define FUR_MULTIPLIER 0.80
		#include "FurHelpers.cginc"
		ENDCG

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
		#define FUR_MULTIPLIER 0.85
		#include "FurHelpers.cginc"
		ENDCG

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
		#define FUR_MULTIPLIER 0.90
		#include "FurHelpers.cginc"
		ENDCG

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
		#define FUR_MULTIPLIER 0.95
		#include "FurHelpers.cginc"
		ENDCG
	}

	Fallback "Diffuse"
}
