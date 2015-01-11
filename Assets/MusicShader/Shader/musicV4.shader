Shader "Custom/Music/Diffusev4" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_Shininess ("Shininess", Range (0.01, 50)) = 0.078125
	_MColor ("Music Color", Color) = (1,1,1,1)
	
	//static texture
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_MusicTexMap(" music(Red)", 2D) = "white" {}
	_Illum ("Illumin (RGB)", 2D) = "black" {}
	_BumpMap ("NormalMap", 2D) = "bump" {}
	_SpecMap ("SpecularMap", 2D) = "specular" {}
	
	//dynamic texture
	_MusicData ("MusicData (Alpha)", 2D) = "white" {} 
	
	
	// from Self-Illumin/Bumped Specular
	//_EmissionLM ("Emission (Lightmapper)", Float) = 0
}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 200

CGPROGRAM
//#pragma surface surf BlinnPhong
#pragma surface surf SimpleSpecular

struct CostumSurfaceOutput {
    half3 Albedo;
    half3 Normal;
    half3 Emission;
    half Specular;
    half3 GlossColor;
    half Alpha;
};

half4 LightingSimpleSpecular (CostumSurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
	half3 h = normalize (lightDir + viewDir);
	
	half diff = max (0, dot (s.Normal, lightDir));
	
	float nh = max (0, dot (s.Normal, h));
	float spec = pow (nh, 48.0*s.Specular);
	
	half4 c;
	c.rgb = (s.Albedo * _LightColor0.rgb * diff + (_LightColor0.rgb * s.GlossColor.rgb * spec)) * (atten * 2);
	c.a = s.Alpha;
	return c;
}

sampler2D _MainTex;
sampler2D _MusicTexMap;
sampler2D _Illum;
sampler2D _BumpMap;
sampler2D _SpecMap;

sampler2D _MusicData;

fixed4 _Color;
fixed4 _MColor;
half _Shininess;

struct Input {
	float2 uv_MainTex;
	float2 uv_MusicTexMap;
	float2 uv_Illum;
	float2 uv_BumpMap;
	float2 uv_SpecMap;
};


void surf (Input IN, inout CostumSurfaceOutput o) {
	
	
	// main texture 
	fixed4 tOut = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	
	// music stuff
	fixed mMapData = (tex2D(_MusicTexMap,IN.uv_MainTex).r);// get music texture input
	
	//if music texture input is 0 nullTestResult is 0 else it's 1
	fixed nullTestData = saturate(mMapData);
	fixed nullTest = round(mMapData);
	fixed nullTestResult = lerp(0,1,nullTestData);
	
	fixed mtOut = ((tex2D(_MusicData, float2((0-mMapData),0)).r*nullTestResult));// get music output data with music texture input data;
	
	fixed3 musicColor = fixed3(mtOut,mtOut,mtOut);//tex2D(_MusicTexMap,float2(musicData,1).a*
	//float3 musicColor = float3(c.r*mMapData.r,c.g*mMapData.r,c.b*mMapData.r);//tex2D(_MusicTexMap,float2(musicData,1).a*
	
	
	o.Albedo = tOut;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	o.Emission = saturate((musicColor*_MColor)+tex2D(_Illum, IN.uv_Illum).rgb);
	o.Specular = _Shininess;
	o.GlossColor = tex2D(_SpecMap, IN.uv_SpecMap).rgb;
	o.Alpha = _Color.a;
}
ENDCG
}

Fallback "Diffuse"
}
