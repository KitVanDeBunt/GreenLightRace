using UnityEngine;
using System.Collections;

public enum MusicMaterialType{
	musicV4,
	musicV5
}

[System.Serializable]
public class MaterialHolder{
	public Material material;
	public MusicMaterialType musicMaterialsType;
	[Range(0.0005f,0.5f)]
	public float delay = 0.0166f;
	public float multiplyer = 1.0f;
	[HideInInspector]
	[System.NonSerialized]
	public float[] spactrumDataDelay;
    [HideInInspector]
    [System.NonSerialized]
    public Texture2D dataTexture;
    public FilterMode filterMode;
}

public class MusicShader : MonoBehaviour {

	//shader materials
	public MaterialHolder[] musicMaterials;
	
	int numSamples = 256;

	void Start(){
		for(int j =0;j<musicMaterials.Length;j++){
            musicMaterials[j].dataTexture = new Texture2D(numSamples, 1, TextureFormat.RGBA32, false);
            musicMaterials[j].dataTexture.filterMode = musicMaterials[j].filterMode;
            musicMaterials[j].material.SetTexture("_MusicData", musicMaterials[j].dataTexture);

			musicMaterials[j].spactrumDataDelay = new float[numSamples];
		}
	}

	void Update() {
        float[] spectrum = new float[numSamples];
        GetComponent<AudioSource>().GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
		
		for(int j =0;j<musicMaterials.Length;j++){
			//float[] spectrum = audio.GetOutputData(numSamples, 0); 
			int i = 1;
			while (i < numSamples+1) {
				float newData =  (spectrum[i - 1]*1.0f*musicMaterials[j].multiplyer);


				//spactrumDataDelay[i-1] = newData;
				if(newData>musicMaterials[j].spactrumDataDelay[i-1]){
					musicMaterials[j].spactrumDataDelay[i-1] += (musicMaterials[j].delay*Time.deltaTime);
					if(musicMaterials[j].spactrumDataDelay[i-1] > newData){
						musicMaterials[j].spactrumDataDelay[i-1] = newData;
					}
				}else{
					musicMaterials[j].spactrumDataDelay[i-1] -= (musicMaterials[j].delay*Time.deltaTime);
					if(musicMaterials[j].spactrumDataDelay[i-1] <0f){
						musicMaterials[j].spactrumDataDelay[i-1] = 0f;
					}
				}
				
				//tex.SetPixel (i + 1, 1, new Color(spectrum[i - 1], 0, 0,1));
				//tex.SetPixel (i - 1 ,1, new Color( spactrumDataDelay[i-1], 0, 0,0));
				//tex.SetPixel (i - 1 ,1, new Color( (spectrum[i - 1]*255.0f*multiplyer), 0, 0,0));

                if (musicMaterials[j].musicMaterialsType == MusicMaterialType.musicV4) {
                    musicMaterials[j].dataTexture.SetPixel(i - 1, 1, new Color((musicMaterials[j].spactrumDataDelay[i - 1] * 255.0f), 0, 0, 0));
				}else{
                    ShaderUtil.WriteFloatToTexturePixel(musicMaterials[j].spactrumDataDelay[i - 1], ref musicMaterials[j].dataTexture, i - 1, 1);
				}
				i++;
			}
			musicMaterials[j].dataTexture.Apply();
		}
	}
}
