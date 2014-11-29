using UnityEngine;
using System.Collections;

public class LightControler : MonoBehaviour
{
	[SerializeField]
	private Light light;
	void OnGUI(){
		light.intensity = GUI.HorizontalSlider(new Rect(20,20,90,20),light.intensity,0f,1.5f);
	}
}

