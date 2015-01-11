Shader "Custom/Music/Diffusev3" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_MusicTexMap(" music(Red)", 2D) = "white" {}
	_MusicData ("MusicData (Alpha)", 2D) = "white" {} 
}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 200

CGPROGRAM
#pragma surface surf Lambert

sampler2D _MainTex;
sampler2D _MusicTexMap;
sampler2D _MusicData;
//sampler2D _MusicSamples;
fixed4 _Color;

struct Input {
	float2 uv_MainTex;
};


void surf (Input IN, inout SurfaceOutput o) {
	
	
	float4 mMapData = tex2D(_MusicTexMap,IN.uv_MainTex);// get music texture input
	float mOut= tex2D(_MusicData, float2((mMapData.r),0));//_MusicDataTex;
	fixed4 c = _Color;
	fixed4 tc = tex2D(_MainTex, IN.uv_MainTex) * c;
	
	float3 musicColor = mOut;//tex2D(_MusicTexMap,float2(musicData,1).a*
	//float3 musicColor = float3(c.r*mMapData.r,c.g*mMapData.r,c.b*mMapData.r);//tex2D(_MusicTexMap,float2(musicData,1).a*
	o.Albedo = tc*musicColor;
	//o.Alpha = 0;
}
ENDCG
}

Fallback "VertexLit"
}
