using UnityEngine;
using System.Collections;

public class WorldPath : MonoBehaviour 
{
	public GameObject[] path;
	private int pathLength;

	void Start () 
	{
		pathLength = path.Length;
	}

	public int getNextID(int currentID)
	{
		currentID++;
		if(currentID > pathLength - 1)
		{
			currentID = 0;
		}
		return currentID;
	}

	public Vector3 getNextPointByID(int id)
	{
		return path[id].transform.position;
	}

	public Node getNodeByID(int id)
	{
		return path[id].GetComponent<Node>();
	}
}
