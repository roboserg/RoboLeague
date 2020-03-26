#pragma target 3.0

fixed4 _Color;
sampler2D _MainTex;
half _Glossiness;
half _Metallic;

uniform float _FurLength;
uniform float _Cutoff;
uniform float _CutoffEnd;
uniform float _EdgeFade;

uniform fixed3 _Gravity;
uniform fixed _GravityStrength;

void vert (inout appdata_full v)
{
	fixed3 direction = lerp(v.normal, _Gravity * _GravityStrength + v.normal * (1-_GravityStrength), FUR_MULTIPLIER);
	v.vertex.xyz += direction * _FurLength * FUR_MULTIPLIER * v.color.a;
}

struct Input {
	float2 uv_MainTex;
	float3 viewDir;
};

void surf (Input IN, inout SurfaceOutputStandard o) {
	fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
	o.Albedo = c.rgb;
	o.Metallic = _Metallic;
	o.Smoothness = _Glossiness;

	//o.Alpha = step(_Cutoff, c.a);
	o.Alpha = step(lerp(_Cutoff,_CutoffEnd,FUR_MULTIPLIER), c.a);

	float alpha = 1 - (FUR_MULTIPLIER * FUR_MULTIPLIER);
	alpha += dot(IN.viewDir, o.Normal) - _EdgeFade;

	o.Alpha *= alpha;
}