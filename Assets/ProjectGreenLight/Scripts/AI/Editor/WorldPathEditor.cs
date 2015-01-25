using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(WorldPath))]
public class WorldPathEditor : Editor 
{
	private Object myTarget;
	//private bool autoUpdate = false;
	
	public override void OnInspectorGUI()
	{
		myTarget = target;
		//autoUpdate = GUILayout.Toggle(autoUpdate,"Auto Update");
		
		if (GUILayout.Button ("Generate Path")) 
		{
			WorldPath path = (WorldPath)myTarget;
			path.Rebuild();
		}
		
		DrawDefaultInspector ();
	}
}