using UnityEngine;
using System.Collections;

public class speedTest : MonoBehaviour {

	public Rigidbody rigidB;
	public float speed = 1f; 
	public float drag = 1000f;

	public bool wdown;
	void Update () {
		//wdown = Input.GetKey (KeyCode.W);
	}

	void FixedUpdate(){
		Debug.Log((rigidB.velocity.magnitude*3.6f));
		//rigidB
		if(wdown){
			rigidB.AddForce((Vector3.up*speed));
		}
		rigidB.AddForce ((rigidB.mass*50f*rigidB.velocity * -1)*((rigidB.velocity.magnitude*rigidB.velocity.magnitude*3.6f)/(1000f)));
	}
}
