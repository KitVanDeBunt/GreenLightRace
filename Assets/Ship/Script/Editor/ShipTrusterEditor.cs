using UnityEngine;
using System.Collections;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(ShipTruster))]
public class ShipTrusterEditor : Editor 
{
	public override void OnInspectorGUI()
	{

		Object[] myTargets = targets;
		
		if(GUILayout.Button("Set Trust Dir"))
		{
			foreach (var truster in myTargets) {
				((ShipTruster)truster).SetTrustDir();
			}
		}
		DrawDefaultInspector();
	}
}