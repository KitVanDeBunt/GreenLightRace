using UnityEngine;
using System.Collections;

public class MusicShaderController : MonoBehaviour
{
	[SerializeField]
	private MusicShader musicShader;

	[SerializeField]
	private Light sunLight;
	
	private float  lightI;
	private float mColorR = 1;
	private float mColorG = 0;
	private float mColorB = 0;
	private float changeTimer = 0;
	
	private GameTheme currentGameTheme;
	private GameTheme oldGameTheme;
	private GameTheme[] gameThemes;
	private int current = 0;
	
	private bool manualControle;
	private bool showController;
	
	void OnGUI(){
		showController = GUI.Toggle (new Rect (10, 10, 180, 30),showController, "Show Music Shader Controller");

		if (GUI.Button (new Rect (260, 10, 180, 30), "Full Screen")) {
			CameraUtils.ToggleFullscreen ();
		}

		if (showController) {
			float checkLightI = lightI;
			float checkRed = mColorR;
			float checkGreen = mColorG;
			float checkBlue = mColorB;
		
			lightI = GUI.HorizontalSlider (new Rect (10, 60, 100, 10), lightI, 0.0f, 1.0f);
			mColorR = GUI.HorizontalSlider (new Rect (10, 80, 100, 10), mColorR, 0.0f, 1f);
			mColorG = GUI.HorizontalSlider (new Rect (10, 90, 100, 10), mColorG, 0.0f, 1f);
			mColorB = GUI.HorizontalSlider (new Rect (10, 100, 100, 10), mColorB, 0.0f, 1f);
		
			if (lightI != checkLightI || mColorR != checkRed || mColorG != checkGreen || mColorB != checkBlue) {
				manualControle = true;
			}
			if (manualControle) {
				if (GUI.Button (new Rect (120, 60, 180, 30), "Auto Color Change(Press T)")) {
					manualControle = false;
					oldGameTheme = new GameTheme (new Color (mColorR, mColorG, mColorB), lightI);
					currentGameTheme = oldGameTheme;
					StartCoroutine (ChangeTheme ());
				}
			} else {
				GUI.TextField (new Rect (120, 60, 130, 30), "<-Manual Controle");
			}
		}

	}

	void Start(){
		manualControle = false;
        currentGameTheme = new GameTheme(new Color(1, 1, 0), 0.03f);
		mColorR = currentGameTheme.color.r;
		mColorG = currentGameTheme.color.g;
		mColorB = currentGameTheme.color.b;
		lightI = currentGameTheme.lightPower;
		oldGameTheme = currentGameTheme;
		gameThemes = new GameTheme[5];
		gameThemes[0] = currentGameTheme;
        gameThemes[1] = new GameTheme(new Color(1, 0, 1), 0.03f);
        gameThemes[2] = new GameTheme(new Color(1, 1, 1), 0.03f);
        gameThemes[3] = new GameTheme(new Color(1, 1, 0), 0.03f);
        gameThemes[4] = new GameTheme(new Color(0, 1, 1), 0.03f);
        gameThemes[4] = new GameTheme(new Color(0, 0.35f, 1), 0.03f);
		sunLight.intensity = lightI;
		
		StartCoroutine(ChangeTheme());
	}

	void Update(){
		if(Input.GetKeyDown(KeyCode.T)){
			if(!manualControle){
				TriggerThemeChange();
			}else{
				manualControle = false;
				oldGameTheme = new GameTheme(new Color(mColorR,mColorG,mColorB),lightI);
				//currentGameTheme = oldGameTheme;
				TriggerThemeChange();
				StartCoroutine(ChangeTheme());
			}
		}
		
		sunLight.intensity = lightI;
		for(int j =0;j<musicShader.musicMaterials.Length;j++){
			musicShader.musicMaterials[j].SetColor("_MColor", new Color(mColorR,mColorG,mColorB));
		}
	}

	IEnumerator UpdateTheme() {
		changeTimer += 0.015f;
		mColorR = Color.Lerp(oldGameTheme.color,currentGameTheme.color,changeTimer).r;
		mColorG = Color.Lerp(oldGameTheme.color,currentGameTheme.color,changeTimer).g;
		mColorB = Color.Lerp(oldGameTheme.color,currentGameTheme.color,changeTimer).b;
		for(int j =0;j<musicShader.musicMaterials.Length;j++){
			musicShader.musicMaterials[j].SetColor("_MColor", new Color(mColorR,mColorG,mColorB));
		}
		lightI = Mathf.Lerp(oldGameTheme.lightPower,currentGameTheme.lightPower,changeTimer);
		yield return new WaitForSeconds(0.025f);
		if(changeTimer < 1){
			yield return StartCoroutine(UpdateTheme());
		}else{
			oldGameTheme = currentGameTheme;
		}
	}
	
	IEnumerator ChangeTheme() {
		if(!manualControle){
			TriggerThemeChange();
			yield return new WaitForSeconds(5f);
			yield return StartCoroutine(ChangeTheme());
		}
	}
	
	void TriggerThemeChange(){
		if(!manualControle){
			if(oldGameTheme.checkSame(currentGameTheme)){
				current++;
				if(current>=gameThemes.Length){
					current = 0;
				}
				currentGameTheme = gameThemes[current];
				changeTimer = 0;
				StartCoroutine(UpdateTheme());
			}
		}
	}
}

