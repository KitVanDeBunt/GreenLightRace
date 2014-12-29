using UnityEngine;
using System.Collections;

public class Reset : MonoBehaviour {
	
	[SerializeField]
	private GameObject resetObject;
	
	[SerializeField]
	private Transform SpawnPoint;
	[SerializeField]
	private Transform SpawnPoint2;
	
	void OnGUI(){
		if(GUI.Button(new Rect(10,50,100,30),"Spawn Track 1")){
			ResetGame(SpawnPoint);
		}
		if(GUI.Button(new Rect(120,50,100,30),"Spawn Track 2")){
			ResetGame(SpawnPoint2);
		}
	}
	
	void Update () {
		if(Input.GetKeyDown(KeyCode.R)){
			ResetGame(SpawnPoint);
		}
	}
	
	public void ResetGame(Transform spawnPoint){
		resetObject.transform.position = spawnPoint.transform.position;
		resetObject.transform.rotation = spawnPoint.transform.rotation;
		rigidbody.velocity = Vector3.zero;
	}
}
