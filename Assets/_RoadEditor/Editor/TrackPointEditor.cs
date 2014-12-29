using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TrackPoint))]
[CanEditMultipleObjects]
public class TrackPointEditor : Editor{

	private Object[] myTargets;

	public override void OnInspectorGUI(){

		EditorApplication.update += (EditorApplication.CallbackFunction)EditortUpdate;
		
		DrawDefaultInspector ();
	}

	public void EditortUpdate(){
		myTargets = targets;
		for (int i = 0; i < myTargets.Length; i++) {
			TrackPoint tempTrack = (TrackPoint)myTargets[i];
			tempTrack.EditortUpdate ();
		}
	}
}

