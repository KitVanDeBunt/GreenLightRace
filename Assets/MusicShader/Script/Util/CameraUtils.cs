using UnityEngine;
using System.Collections;

static class CameraUtils {

	static public void ToggleFullscreen(){
		if(Screen.fullScreen == true){
			Screen.fullScreen = false;
		}else{
			Screen.fullScreen = true;
		}
	}
}
