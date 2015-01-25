using UnityEngine;
using System.Collections;

public class LightControler : MonoBehaviour
{
	[SerializeField]
	private Light myLight;
	void OnGUI(){
        myLight.intensity = GUI.HorizontalSlider(new Rect(20, 20, 90, 20), myLight.intensity, 0f, 1.5f);
	}
}

