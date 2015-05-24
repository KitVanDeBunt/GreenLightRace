using UnityEngine;
using System.Collections;

public class ShipTruster : MonoBehaviour {

	[SerializeField]
	private Vector3 direction;
	[SerializeField]
	private float force = 1000f;
	[SerializeField]
	private float forceMult = 2f;

	public float Force {
		get {
			return force*forceMult;
		}
	}

	public Vector3 Direction {
		get {
			return direction;
		}
		set{
			direction = value;
		}
	}

	void OnDrawGizmos(){
	//void OnDrawGizmosSelected(){
		//draw thrust line
		Gizmos.color = Color.red;
		Gizmos.DrawRay (transform.position,direction);
	}

	public void SetTrustDir(){
		direction = Vector3.up;
	}
}
