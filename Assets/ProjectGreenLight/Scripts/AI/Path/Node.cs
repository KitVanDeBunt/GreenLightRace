using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Node : MonoBehaviour 
{
    public Transform maxLeft;
    public Transform maxRight;
	public Transform center;

	public float maxSpeed;
	public float minDist;

	public Node next;
	public Node previous;

	[SerializeField]
	[HideInInspector]
	private bool generateLabels;

	public bool GenerateLabels {
		set {
			generateLabels = value;
			InitPoints();
		}
	}

	void Awake () 
	{
		InitPoints();
		minDist = Vector2.Distance(new Vector2(maxRight.position.x, maxRight.position.z), new Vector2(maxLeft.position.x, maxLeft.position.z));
		minDist *= 1.2f;
	}
	
	private void InitPoints(){
		InitPoint(ref maxLeft,"left");
		InitPoint(ref maxRight,"right");
		InitPoint(ref center,"center");
	}
	
	private void InitPoint(ref Transform point, string name){
		if(point==null)
		{
			point = transform.Find(name);
			if(point == null)
			{
				point = new GameObject(name).transform;
				point.transform.parent = transform;
				point.transform.position = transform.position;
				point.transform.rotation = transform.rotation;
			}
		}
		#if UNITY_EDITOR
		if(generateLabels){
			if (name == "left")
			{
				IconManager.SetIcon(point.gameObject, IconManager.Icon.DiamondYellow);
			}
			else if (name == "right")
			{
				IconManager.SetIcon(point.gameObject, IconManager.Icon.DiamondOrange);
			}
			else if (name == "center")
			{
				IconManager.SetIcon(point.gameObject, IconManager.LabelIcon.Yellow);
			}
		}
		#endif
	}
	
	public bool checkDistance(Vector2 carPos)
	{
		if(Vector2.Distance(new Vector2(maxRight.position.x, maxRight.position.z), carPos) + Vector2.Distance(new Vector2(maxLeft.position.x, maxLeft.position.z), carPos) < minDist)
		{
			return true;
		}
		return false;
	}
}
