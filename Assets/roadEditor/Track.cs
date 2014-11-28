using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Track : MonoBehaviour {
	
	[SerializeField]
	private int roadLength = 1;
	[SerializeField]
	private int roadSize = 30;
	[SerializeField]
	private Material trackMaterial;
	[SerializeField]
	private float pointsPerUnityUnit = 1;
	[SerializeField]
	private int widthDetail = 1;
	[SerializeField]
	private Transform[] pointsNew;
	[SerializeField]
	private string name = "road";

	[HideInInspector]
	[SerializeField]
	private Vector3[] pointsCurrent;
	[HideInInspector]
	[SerializeField]
	private Quaternion[] rotationsCurrent;

	[HideInInspector]
	[SerializeField]
	private Vector3[] pointsCurrentSource;
	[HideInInspector]
	[SerializeField]
	private Quaternion[] pointsCurrentRotSource;

	private Mesh trackMesh;
	[SerializeField]
	private GameObject pointHolder;

	private List<Vector3> trackVertices;
	private List<Vector2> trackUvs;
	private List<int> trackTriangles;
	private int i;
	private int j;
	private int k;
	private int l;
	private float t;
	
	void OnGUI(){
		if(GUI.Button(new Rect(20,20,90,20),"Redraw")){
			Draw ();
		}
	}

	void Start () {
		Draw ();
	}

	int oneIn50 = 0;
	public void EditortUpdate(){
		oneIn50++;
		if (oneIn50 % 50 == 0) {
			UpdateTrackIfNeeded ();
		}
	}

	void FixedUpdate(){
		UpdateTrackIfNeeded ();
	}

	void UpdateTrackIfNeeded (){
		bool redo =  false;
		if (pointsNew.Length != pointsCurrentSource.Length) {
			redo = true;
		} else if(!redo){
			for (i = 0; i<pointsNew.Length; i++) {
				if(pointsNew[i].position!=pointsCurrentSource[i]){
					redo = true;
					break;
				}
			}
		}
		if (!redo) {
			if (pointsNew.Length != pointsCurrentRotSource.Length) {
					redo = true;
			} else if (!redo) {
				for (i = 0; i<pointsNew.Length; i++) {
					if (pointsNew[i].rotation != pointsCurrentRotSource [i]) {
						redo = true;
						break;
					}
				}
			}
		}

		if (redo) {
			Debug.Log("redo");
			Draw ();
		}
	}

	void CreatePoints (){
		if (pointsNew.Length < 1) {
			Debug.LogError("need more then 1 point!!");
		}
		if (pointsNew.Length < 2) {
			Debug.LogError("need more then 2 points!!");// temoprery solution
		}

		List<Vector3> pointsTemp = new List<Vector3> (); 
		List<Quaternion> rotTemp = new List<Quaternion> (); 
		
		for (i = 0; i<pointsNew.Length; i++) {
			pointsNew[i].name = name+" p: "+i;
			pointsTemp.Add(pointsNew[i].position);
			rotTemp.Add(pointsNew[i].rotation);
		}
		List<Vector3> pointsBeziered = new List<Vector3> ();
		List<Quaternion> rotationsBeziered = new List<Quaternion> ();
		int bezierLevels = pointsTemp.Count +1 -2;
		float bezierApproximateLength = 0;
		for (i = 0; i < bezierLevels; i++) {
			bezierApproximateLength += Vector3.Distance( pointsTemp[i],pointsTemp[i+1]);
		}
		float bezierSubsections = bezierApproximateLength * pointsPerUnityUnit;
		float bezierTStepSize = 1f / bezierSubsections;
		/*
		Debug.Log ("bezierSubsections: "+bezierSubsections);
		Debug.Log ("bezierLevels: "+bezierLevels);
		Debug.Log ("bezierApproximateLength: "+bezierApproximateLength);
		Debug.Log ("bezierTStepSize: "+bezierTStepSize);
		*/
		//for (i = 0; i < pointsTemp.Count; i++) {
		for(t = 0; t < 1;t+=bezierTStepSize){
			Vector3[] levelPositions = new Vector3[bezierLevels];
			Vector3[] levelPositionsNew;
			
			Quaternion[] levelRotations = new Quaternion[bezierLevels];
			Quaternion[] levelRotationsNew;
			for(j = 0; j < bezierLevels+1; j++){
				int levelPointCalculationLength = ((bezierLevels-1)-j);
				//Debug.Log("levelPointCalculationLength: "+levelPointCalculationLength);
				//for(k = 0; k < levelPointsLength; k++){
				if(j == 0){//first bezierLevels
					for(l = 0; l < levelPointCalculationLength+1; l++){
						//Debug.Log("levelPCL: "+l);
						levelRotations[l] = Quaternion.Lerp(rotTemp[l],rotTemp[l+1],t);
						levelPositions[l] = Vector3.Lerp(pointsTemp[l],pointsTemp[l+1],t);
					}
				}else if(j == bezierLevels){//last bezierLevels
					//Debug.Log("add:"+levelPositions[0]);
					pointsBeziered.Add(levelPositions[0]);
					rotationsBeziered.Add(levelRotations[0]);
					//Debug.Log("add: "+l);
				}else{// in between bezierLevels
					levelPositionsNew = new Vector3[bezierLevels];
					levelRotationsNew = new Quaternion[bezierLevels];
					for(l = 0; l < levelPointCalculationLength+1; l++){
						//Debug.Log("level--- btw: "+l);
						levelPositionsNew[l] = Vector3.Lerp(levelPositions[l],levelPositions[l+1],t);
						levelRotationsNew[l] = Quaternion.Lerp(levelRotations[l],levelRotations[l+1],t);
						//Debug.Log("p:"+l+": "+levelPositionsNew[l]);
					}
					levelRotations = levelRotationsNew;
					levelPositions = levelPositionsNew;
				}
				//}
			}
		}
		//}
		//for (i = 0; i < pointsTemp.Count; i++) {
		//}
		//add point t 1
		pointsBeziered.Add(Vector3.Lerp(pointsTemp[pointsTemp.Count-2],pointsTemp[pointsTemp.Count-1],1));
		rotationsBeziered.Add(Quaternion.Lerp(rotTemp[rotTemp.Count-2],rotTemp[rotTemp.Count-1],1));

		pointsCurrent = pointsBeziered.ToArray ();
		rotationsCurrent = rotationsBeziered.ToArray ();
		pointsCurrentSource = pointsTemp.ToArray ();
		pointsCurrentRotSource = rotTemp.ToArray ();
		//Debug.Log("pointsCurrent.length: "+pointsCurrent.Length);
		//for (i = 0; i < pointsCurrent.Length; i++) {
		//	Debug.Log(i+":"+pointsCurrent[i]);
		//}
	}
	
	void Draw (){
		CreatePoints ();

		roadLength = pointsCurrent.Length - 1;
		float roadPartLength = roadLength*widthDetail;

		if (pointHolder == null) {
			bool pointFound = false;
			for(i = 0;i<gameObject.transform.childCount;i++){
				if(gameObject.transform.GetChild(i).name == "pointHolder"){
					pointHolder = gameObject.transform.GetChild(i).gameObject;
					pointFound = true;
				}
			}
			if(!pointFound){
				pointHolder = new GameObject("pointHolder");
				pointHolder.transform.parent = gameObject.transform;
			}
		}

		MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
		if(meshFilter==null){
			meshFilter = gameObject.AddComponent<MeshFilter>();
		}
		
		MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
		if(meshRenderer==null){
			meshRenderer = gameObject.AddComponent<MeshRenderer>();
		}
		
		MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
		if(meshCollider==null){
			meshCollider = gameObject.AddComponent<MeshCollider>();
		}
		
		trackMesh = new Mesh();
		trackMesh.name = "roadMesh";
		
		//Vertices
		trackVertices = new List<Vector3>();

		
		float[] trackX = new float[widthDetail*2];
		float widthPartSize = 1.0f/(float)widthDetail;
		float currentXPart = -0.5f;
		for(j = 0;j < trackX.Length;j+=2){
			trackX[j] = currentXPart;
			currentXPart+=widthPartSize;
			trackX[j+1] = currentXPart;
		}

		for(i = 0;i < roadLength;i++){

			Vector3 pointPos = pointsCurrent[i]- transform.position;
			Vector3 pointPosNex = pointsCurrent[i+1]- transform.position;
			
			Vector3 newVert;
			
			for(j = 0;j < trackX.Length;j+=2){
				newVert = pointPos+(rotationsCurrent[i]*new Vector3(trackX[j]*roadSize,0,0));
				trackVertices.Add(newVert);
	
				newVert = pointPos+(rotationsCurrent[i]*new Vector3(trackX[j+1]*roadSize,0,0));
				trackVertices.Add(newVert);
	
				newVert = pointPosNex+(rotationsCurrent[i+1]*new Vector3(trackX[j]*roadSize,0,0));
				trackVertices.Add(newVert);
	
				newVert = pointPosNex+(rotationsCurrent[i+1]*new Vector3(trackX[j+1]*roadSize,0,0));
				trackVertices.Add(newVert);
			}
		}
		
		trackMesh.vertices = trackVertices.ToArray();
		
		//uv
		trackUvs = new List<Vector2>();
		
		for(i = 0;i < roadPartLength;i++){
			trackUvs.Add(new Vector2( 0.0f,0.0f));
			trackUvs.Add(new Vector2( 1.0f,0.0f));
			trackUvs.Add(new Vector2( 0.0f,1.0f));
			trackUvs.Add(new Vector2( 1.0f,1.0f));
		}
		
		trackMesh.uv = trackUvs.ToArray();
		
		//triangles
		trackTriangles = new List<int>();
		
		for(i = 0;i < roadPartLength;i++){
			trackTriangles.Add((1+(4*i)));
			trackTriangles.Add((0+(4*i)));
			trackTriangles.Add((3+(4*i)));
			trackTriangles.Add((0+(4*i)));
			trackTriangles.Add((2+(4*i)));
			trackTriangles.Add((3+(4*i)));
			//1,0,3,
			//0,2,3
		}
		
		trackMesh.triangles = trackTriangles.ToArray();
		
		//
		trackMesh.Optimize();
		trackMesh.RecalculateNormals();
		meshRenderer.material = trackMaterial;
		
		meshFilter.mesh = trackMesh;
		meshCollider.sharedMesh = meshFilter.sharedMesh;
	}
}
