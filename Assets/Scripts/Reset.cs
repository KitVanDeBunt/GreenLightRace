using UnityEngine;
using System.Collections;

public class Reset : MonoBehaviour {
	
	[SerializeField]
	private GameObject resetObject;
	
	[SerializeField]
	private GameObject SpawnPoint;
	
	void Update () {
		if(Input.GetKeyDown(KeyCode.R)){
			ResetGame();
		}
	}
	
	public void ResetGame(){
		resetObject.transform.position = SpawnPoint.transform.position;
		resetObject.transform.rotation = SpawnPoint.transform.rotation;
		rigidbody.velocity = Vector3.zero;
	}
}
