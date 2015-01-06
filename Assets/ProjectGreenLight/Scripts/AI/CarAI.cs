using UnityEngine;
using System.Collections;

public class CarAI : MonoBehaviour 
{
	private WorldPath worldpath;
	private int nextPointID = 0;
	private Vector3 nextPoint;
	private Node nextNode;

	private CarControl2 control;

	void Start () 
	{
		worldpath = GameObject.Find("World").GetComponent<WorldPath>();
		control = gameObject.GetComponent<CarControl2> ();

		nextPoint = worldpath.getNextPointByID(nextPointID);
		nextNode = worldpath.getNodeByID(nextPointID);
	}

	void Update () 
	{
		Vector3 _direction = (nextPoint - transform.position).normalized;
		Quaternion _lookRotation = Quaternion.LookRotation(_direction);
		_lookRotation = Quaternion.Euler(0,_lookRotation.eulerAngles.y,0);

		float r = fixAngle(_lookRotation.eulerAngles.y - fixAngle(transform.rotation.eulerAngles.y));

		if(control.Speed < nextNode.maxSpeed)
		{
			control.aiInputMoter = 1f;
		}
		else
		{
			control.aiInputMoter = 0f;
		}

		if(Mathf.Abs(r) < 10)
		{
			control.aiInputSteer = maxSteer(r / 15f);
		}
		else
		{
			control.aiInputSteer = maxSteer(r);
		}

		if(nextNode.checkDistance(new Vector2(transform.position.x, transform.position.z)))
		{
			nextPointID = worldpath.getNextID(nextPointID);
			nextPoint = worldpath.getNextPointByID(nextPointID);
			nextNode = worldpath.getNodeByID(nextPointID);
		}

		//Debug.Log(control.aiInputSteer + " " + r);
	}

	private float maxSteer(float a)
	{
		if(a < -1)
		{
			return -1;
		}
		if(a > 1)
		{
			return 1;
		}
		return a;
	}

	private float fixAngle(float a)
	{
		if(a > 180)
		{
			a -= 360;
		}
		return a;
	}
}
