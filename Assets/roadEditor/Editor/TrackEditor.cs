using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Track))]
[CanEditMultipleObjects]
public class TrackEditor : Editor {
	private Object[] myTargets;
	private Track tempTrack;
	private bool worldMatrix = true;
	private bool correctedXAxisExport = true;
	private int i;

	public override void OnInspectorGUI(){
		myTargets = targets;

		string filePath = EditorGUILayout.TextField("file path:",(Application.dataPath+"/RoadEditor/Exports/"));
		
		
		string multiText =  "";
		if(myTargets.Length>1){
			multiText +="(multiSelected)";
		}
		worldMatrix = GUILayout.Toggle(worldMatrix,"Use World Space Positions");
		correctedXAxisExport = GUILayout.Toggle(correctedXAxisExport,"export with inverted x-axis ");
		// export
		if (GUILayout.Button ("Export OBJ"+multiText)) {
			for (int i = 0; i < myTargets.Length; i++) {
				tempTrack = (Track)myTargets[i];
				ObjExporter.MeshToFile (tempTrack.gameObject.GetComponent<MeshFilter> (), filePath, (tempTrack.getName + ".obj"),correctedXAxisExport, worldMatrix);
			}
		}

		EditorApplication.update += (EditorApplication.CallbackFunction)EditortUpdate;

		DrawDefaultInspector ();
	}
	
	// calls EditortUpdate on all selected tracks
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