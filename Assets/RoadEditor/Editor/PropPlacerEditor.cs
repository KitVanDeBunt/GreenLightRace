using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(PropPlacer))]
public class PropPlacerEditor : Editor 
{
	private Object myTarget;
	//private bool autoUpdate = false;
	
	public override void OnInspectorGUI()
	{
		myTarget = target;
		//autoUpdate = GUILayout.Toggle(autoUpdate,"Auto Update");
		
		if (GUILayout.Button ("Generate Props")) 
		{
            PropPlacer path = (PropPlacer)myTarget;
			path.Rebuild();
		}
		
		DrawDefaultInspector ();
	}
}