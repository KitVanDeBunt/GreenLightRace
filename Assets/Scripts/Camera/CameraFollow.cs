using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour 
{
	public Transform target;
	private Transform camera;

	private float posX = 0f;
	private float posY = 0f;

	private float height = 2f;
	private float distance = 6f;

	void Start () 
	{
		camera = target;

		Vector3 a = camera.eulerAngles;
		posX = a.x;
		posY = a.y;

		if(this.rigidbody != null)
		{
			rigidbody.freezeRotation = true;
		}
	}

	void LateUpdate () 
	{
		float tA = target.eulerAngles.y;
		float cA = camera.eulerAngles.y;
		posX = Mathf.LerpAngle(cA, tA, 3f * Time.deltaTime);

		Quaternion rot = Quaternion.Euler(posX, posY, 0);
		Vector3 targetMod = new Vector3 (0, height, 0);

		Vector3 pos = target.position - (rot * Vector3.forward * (distance) + targetMod);
		//Vector3 pos2 = target.position - (rot * Vector3.forward * (0.1) + targetMod);

		camera.rotation = rot;
		camera.position = pos;
	}
}
