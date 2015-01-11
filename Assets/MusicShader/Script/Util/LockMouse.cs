using UnityEngine;
using System.Collections;

public class LockMouse : MonoBehaviour{

	public GUIText mouseLockText;

	void Update (){
		if(Input.GetKeyDown(KeyCode.M)){
			ToggleLock();
		}
	}

	void ToggleLock(){
		Screen.lockCursor = !Screen.lockCursor;
		if(Screen.lockCursor){
			mouseLockText.text = "";
		}else{
			mouseLockText.text = "press m to lock/unlock mouse";
		}
	}
}

