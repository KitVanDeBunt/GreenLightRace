using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Track))]
public class TrackEditor : Editor 
{
	Track myTarget;
	public override void OnInspectorGUI(){
		myTarget = (Track)target;
		string filename;
		EditorGUILayout.LabelField("Track Editor");
		string filePath = EditorGUILayout.TextField("file path:",(Application.dataPath+"/RoadEditor/Exports/"));
		//string fileName = EditorGUILayout.TextField("file name:",myTarget.name);
		//fileName+= ".obj";
		if(GUILayout.Button("Export OBJ")) {
			ObjExporter.MeshToFile( myTarget.gameObject.GetComponent<MeshFilter>(),filePath,(myTarget.getName+".obj"));
		}
		EditorApplication.update += (EditorApplication.CallbackFunction)EditortUpdate;

		DrawDefaultInspector ();
	}

	public void EditortUpdate(){
		Repaint ();
		
		myTarget.EditortUpdate();
	}
}