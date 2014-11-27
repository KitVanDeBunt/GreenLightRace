using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Track))]
public class TrackEditor : Editor 
{
	public override void OnInspectorGUI(){
		EditorGUILayout.LabelField("Track Editor");

		EditorApplication.update += (EditorApplication.CallbackFunction)EditortUpdate;

		DrawDefaultInspector ();
	}

	public void EditortUpdate(){
		Repaint ();
		Track myTarget = (Track)target;
		myTarget.EditortUpdate();
	}
}