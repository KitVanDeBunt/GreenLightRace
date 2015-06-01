Shader "Custom/Music/Diffusev4.2" {
	Properties {
		//_Color ("Main Color", Color) = (1,1,1,1)
		//_MColor ("Music Color", Color) = (1,1,1,1)
		
		//static texture
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		//_MusicTexMap(" music(Red)", 2D) = "white" {}
		//_Illum ("Illumin (RGB)", 2D) = "black" {}
		
		//dynamic texture
		//_MusicData ("MusicData (Alpha)", 2D) = "white" {} 
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			//sampler2D _MusicTexMap;
			//sampler2D _MusicData;

			//fixed4 _Color;
			//fixed4 _MColor;
			//half _Shininess;


			fixed4 frag(v2f_img i) : SV_Target {
			    return tex2D(_MainTex, i.uv);
			}
			ENDCG
		}
	}
	//Fallback "Diffuse"
}
