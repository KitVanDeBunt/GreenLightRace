using UnityEngine;
using System.Collections;

public class Node : MonoBehaviour 
{
	public GameObject maxLeft;
	public GameObject maxRight;

	public float maxSpeed;
	public float minDist;

	void Start () 
	{
		minDist = Vector2.Distance(new Vector2(maxRight.transform.position.x, maxRight.transform.position.z), new Vector2(maxLeft.transform.position.x, maxLeft.transform.position.z));
		minDist *= 1.2f;
	}

	public bool checkDistance(Vector2 carPos)
	{
		if(Vector2.Distance(new Vector2(maxRight.transform.position.x, maxRight.transform.position.z), carPos) + Vector2.Distance(new Vector2(maxLeft.transform.position.x, maxLeft.transform.position.z), carPos) < minDist)
		{
			return true;
		}
		return false;
	}
}
