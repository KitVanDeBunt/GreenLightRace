using UnityEngine;
using System.Collections;

public class MusicShader : MonoBehaviour {

	//shader materials
	public Material[] musicMaterials;
	[SerializeField]
	private Material testMat;
	[SerializeField]
	private Material testMat2;
	
	private Texture2D tex;
	
	int numSamples = 256;
	
	[SerializeField]
	private float multiplyer = 1;

	
	void Start(){
		
		tex = new Texture2D (numSamples, 1, TextureFormat.RGBA32, false);
		//renderer.sharedMaterial = mat;
		//mat.SetFloat("_MusicSamples", numSamples);
		testMat.SetTexture ("_MainTex", tex);
		testMat2.SetTexture ("_MainTex", tex);
		for(int j =0;j<musicMaterials.Length;j++){
			musicMaterials[j].SetTexture ("_MusicData", tex);
		}
	}

	void Update() {

		float[] spectrum = audio.GetSpectrumData(numSamples, 0, FFTWindow.BlackmanHarris);
		//float[] spectrum = audio.GetOutputData(numSamples, 0); 
		int i = 1;
		while (i < numSamples+1) {
			//tex.SetPixel (i + 1, 1, new Color(spectrum[i - 1], 0, 0,1));
			tex.SetPixel (i - 1 ,1, new Color( (spectrum[i - 1]*255.0f*multiplyer), 0, 0,0));
			i++;
		}
		tex.Apply ();
	}
}
