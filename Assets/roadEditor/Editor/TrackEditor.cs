using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Track))]
[CanEditMultipleObjects]
public class TrackEditor : Editor {
	private Object[] myTargets;
	private Track tempTrack;
	private int i;

	public override void OnInspectorGUI(){
		myTargets = targets;
		//myTarget = (Track)target;

		string filename;
		EditorGUILayout.LabelField("Track Editor");
		string filePath = EditorGUILayout.TextField("file path:",(Application.dataPath+"/RoadEditor/Exports/"));
		//string fileName = EditorGUILayout.TextField("file name:",myTarget.name);
		//fileName+= ".obj";

		string multiButton =  "";
		if(myTargets.Length>1){
			multiButton +="(multiSelected)";
		}
		if (GUILayout.Button ("Export OBJ"+multiButton)) {
			for (int i = 0; i < myTargets.Length; i++) {
				tempTrack = (Track)myTargets[i];
				ObjExporter.MeshToFile (tempTrack.gameObject.GetComponent<MeshFilter> (), filePath, (tempTrack.getName + ".obj"));
			}
		}
		if (GUILayout.Button ("Export OBJ V2"+multiButton)) {
			for (int i = 0; i < myTargets.Length; i++) {
				tempTrack = (Track)myTargets[i];
				ObjExporterV2.MeshToFile (tempTrack.gameObject.GetComponent<MeshFilter> (), filePath, (tempTrack.getName + ".obj"));
			}
		}

		EditorApplication.update += (EditorApplication.CallbackFunction)EditortUpdate;

		DrawDefaultInspector ();
	}

	public void EditortUpdate(){
		Repaint ();
		myTargets = targets;
		for (int i = 0; i < myTargets.Length; i++) {
			tempTrack = (Track)myTargets[i];
			if(tempTrack!=null){
				tempTrack.EditortUpdate ();
			}
		}
	}
}