using UnityEngine;
using System.Collections;

public class CarAI : MonoBehaviour 
{
	private Vector3 nextPoint;
	private CarControl2 control;

	void Start () 
	{
		control = gameObject.GetComponent<CarControl2> ();
		nextPoint = new Vector3(1, 0, 1);
	}

	void Update () 
	{
		float currentDirection = 0.0F;
		Vector3 currentAxis = Vector3.zero;
		transform.rotation.ToAngleAxis(out currentDirection, out currentAxis);
		/*
		float targetDirection = Vector3.Angle(transform.position, nextPoint);
		targetDirection = Vector2.Angle (new Vector2 (transform.position.x, transform.position.z), new Vector2 (0, 0));

		Vector3 test = transform.position - nextPoint;
		float a = Mathf.Atan2(test.y, test.x) * 180f / Mathf.PI;

		Debug.Log (a);*/


		float test = Mathf.Atan2((nextPoint.z - transform.position.z), (nextPoint.x - transform.position.x)) * Mathf.Rad2Deg;

		//test += currentDirection;

		Debug.Log(((-test) + (currentDirection - 90)));

	}
}
