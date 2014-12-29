using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[ExecuteInEditMode]
public class Track : MonoBehaviour {
	
	[SerializeField]
	private bool updateInGame = false;
	
	[SerializeField]
	private TrackSettings trackSettings;
	
	enum TrackSide{
		Left = 0,
		Right = 1
	}
	[SerializeField]
	private Transform[] pointsNew;

	[HideInInspector]
	[SerializeField]
	private List<Transform> pointsList;

	[HideInInspector]
	[SerializeField]
	private Vector3[] currentPositionsBeziered;
	[HideInInspector]
	[SerializeField]
	private Quaternion[] currentRotations;
	[HideInInspector]
	[SerializeField]
	private float[] currentWidths;

	[HideInInspector]
	[SerializeField]
	private Vector3[] currentPositionsSource;
	[HideInInspector]
	[SerializeField]
	private Quaternion[] currentRottationsSource;
	[HideInInspector]
	[SerializeField]
	private float[] currentWidthsSource;

	[SerializeField]
	private GameObject pointHolder;
	
	private Mesh tempTrackMesh;
	private bool needMorePoints;
	private bool needSetPointsSize;
	private List<Vector3> trackVertices;
	private List<Vector2> trackUvs;
	private List<int> trackTriangles;
	private float roadPartLength;
	private int roadLength = 1;
	private int i;
	private int j;
	private int k;
	private int l;
	private float t;
	private int updateTimer = 0;
	
	public string getName{
		get{
			return name;
		}
	}
	
	void Start () {
		needMorePoints = false;
		needSetPointsSize = false;
		Loop();
	}
	
	public void EditortUpdate(){
		updateTimer++;
		if (updateTimer % 100 == 0) {
			if(!Application.isPlaying){
				Loop();
			}
		}
	}

	void FixedUpdate(){
		if(updateInGame){
			Loop ();
		}
	}

	void Loop(){
		bool updateTrack = false;
		if (needMorePoints) {
			if(pointsNew.Length>1){
				needMorePoints = false;
				updateTrack = true;
			}
		} else {
			updateTrack = true;
		}

		if (pointsNew == null) {
			if(!needSetPointsSize){
				Debug.Log("Set Points New Size");
				needSetPointsSize = true;
			}
			updateTrack = false;
		}else{
			needSetPointsSize = false;
			if(updateTrack){
				UpdateTrackIfNeeded ();
			}
		}
	}

	void AddPoint(int number){
		pointsNew [number] = new GameObject("new point").transform;
		pointsNew [number].transform.position = pointHolder.transform.position;
		pointsNew [number].gameObject.AddComponent<TrackPoint>();
		if (pointsList == null) {
			pointsList = new List<Transform>();
		}
		pointsList.Add (pointsNew [number]);
	}

	void UpdateTrackIfNeeded (){
		bool redo =  false;
		
		// create or find pointholder
		if (pointHolder == null) {
			bool pointHolderFound = false;
			for(i = 0;i<gameObject.transform.childCount;i++){
				if(gameObject.transform.GetChild(i).name == "pointHolder"){
					pointHolder = gameObject.transform.GetChild(i).gameObject;
					pointHolderFound = true;
				}
			}
			if(!pointHolderFound){
				pointHolder = new GameObject("pointHolder");
				pointHolder.transform.position = gameObject.transform.position;
				pointHolder.transform.parent = gameObject.transform;
			}
		}

		// add new points
		for (i = 0; i<pointsNew.Length; i++) {
			if(pointsNew [i]==null){
				AddPoint(i);
			}
#if UNITY_EDITOR
			IconManager.SetIcon (pointsNew [i].gameObject, trackSettings.labelType);
#endif
			for (j = 0; j<pointsNew.Length; j++) {
				if(i!=j){
					if(pointsNew[i]==pointsNew[j]){
						AddPoint(i);
					}
				}
			}
			pointsNew [i].transform.parent = pointHolder.transform;
		}
		
		// check point length change
		if (pointsNew.Length != currentPositionsSource.Length) {
			redo = true;
		}
		
		// check position change
		if(!redo){
			for (i = 0; i<pointsNew.Length; i++) {
				if(pointsNew[i].position!=currentPositionsSource[i]){
					redo = true;
					break;
				}
			}
		}
		
		// check rotaition change
		if (!redo) {
			if (pointsNew.Length != currentRottationsSource.Length) {
					redo = true;
			} else if (!redo) {
				for (i = 0; i<pointsNew.Length; i++) {
					if (pointsNew[i].rotation != currentRottationsSource [i]) {
						redo = true;
						break;
					}
				}
			}
		}
		
		// check width change
		if (!redo) {
			if (pointsNew.Length != currentWidthsSource.Length) {
				redo = true;
			} else if (!redo) {
				for (i = 0; i<pointsNew.Length; i++) {
					if (pointsNew[i].GetComponent<TrackPoint>().width != currentWidthsSource [i]) {
						redo = true;
						break;
					}
				}
			}
		}
		
		// check settings change
		if (!redo) {
			if(trackSettings.CheckUpdated()){
				redo = true;
				trackSettings.Save();
			}
		}
		
		// update track if needed
		if (redo) {
		
			//delete old points
			int pointCount = pointsList.Count;
			for (i = pointCount - 1; i > 0; i--) {
				bool pointinUse = false;
				for (j = 0; j<pointsNew.Length; j++) {
					if(pointsList[i]==pointsNew[j]){
						pointinUse = true;
						break;
					}
				}
				
				if(!pointinUse){
					if(pointsList[i]!=null && pointsList[i].gameObject!=null){
						if(Application.isEditor){
							GameObject.DestroyImmediate(pointsList[i].gameObject);
						}else{
							GameObject.Destroy(pointsList[i]);
						}
					}
					pointsList.Remove(pointsList[i]);
				}
			}
			Debug.Log("update track");
			Draw ();
		}
	}
	
	void Draw (){
		bool created = CreateBezieredPoints ();
		if (created) {
			CreateMesh ();
		}
	}

	bool CreateBezieredPoints (){
		if (pointsNew.Length < 2) {
			Debug.LogError("need more then 1 point!!");
			needMorePoints = true;
			return false;
		}
		
		//create temporary source point lists
		List<Vector3> tempListPos = new List<Vector3> (); 
		List<Quaternion> tempListRot = new List<Quaternion> (); 
		List<float> tempListWidth = new List<float> (); 
		for (i = 0; i<pointsNew.Length; i++) {
			pointsNew[i].name = name+" p: "+i;
			tempListPos.Add(pointsNew[i].position);
			tempListRot.Add(pointsNew[i].rotation);
			TrackPoint trackPoint = pointsNew[i].GetComponent<TrackPoint>();
			tempListWidth.Add(trackPoint.width);
		}
		
		//create temporary beziered point lists
		List<Vector3> pointsBeziered = new List<Vector3> ();
		List<Quaternion> rotationsBeziered = new List<Quaternion> ();
		List<float> widthBeziered = new List<float> ();
		
		// create other variables needed
		int bezierLevels = tempListPos.Count +1 -2;
		float bezierApproximateLength = 0;
		for (i = 0; i < bezierLevels; i++) {
			bezierApproximateLength += Vector3.Distance( tempListPos[i],tempListPos[i+1]);
		}
		float bezierSubsections = bezierApproximateLength * trackSettings.pointsPerUnityUnit;
		float bezierTStepSize = 1f / bezierSubsections;
		
		/*
		Debug.Log ("bezierSubsections: "+bezierSubsections);
		Debug.Log ("bezierLevels: "+bezierLevels);
		Debug.Log ("bezierApproximateLength: "+bezierApproximateLength);
		Debug.Log ("bezierTStepSize: "+bezierTStepSize);
		*/
		
		// create beziered point from points
		for(t = 0; t < 1;t+=bezierTStepSize){
			Vector3[] levelPositions = new Vector3[bezierLevels];
			Vector3[] levelPositionsNew;
			
			Quaternion[] levelRotations = new Quaternion[bezierLevels];
			Quaternion[] levelRotationsNew;
			
			float[] levelWidth = new float[bezierLevels];
			float[] levelWidthNew;
			
			
			for(j = 0; j < bezierLevels+1; j++){
				int levelPointCalculationLength = ((bezierLevels-1)-j);
				//first bezierLevels
				if(j == 0){
					for(l = 0; l < levelPointCalculationLength+1; l++){
						levelRotations[l] = Quaternion.Lerp(tempListRot[l],tempListRot[l+1],t);
						levelPositions[l] = Vector3.Lerp(tempListPos[l],tempListPos[l+1],t);
						levelWidth[l] = Mathf.Lerp(tempListWidth[l],tempListWidth[l+1],t);
					}
				//last bezierLevels
				}else if(j == bezierLevels){
					pointsBeziered.Add(levelPositions[0]);
					rotationsBeziered.Add(levelRotations[0]);
					widthBeziered.Add(levelWidth[0]);
				// in between bezierLevels
				}else{
					levelPositionsNew = new Vector3[bezierLevels];
					levelRotationsNew = new Quaternion[bezierLevels];
					levelWidthNew = new float[bezierLevels];
					for(l = 0; l < levelPointCalculationLength+1; l++){
						levelPositionsNew[l] = Vector3.Lerp(levelPositions[l],levelPositions[l+1],t);
						levelRotationsNew[l] = Quaternion.Lerp(levelRotations[l],levelRotations[l+1],t);
						levelWidthNew[l] = Mathf.Lerp(levelWidth[l],levelWidth[l+1],t);
					}
					levelRotations = levelRotationsNew;
					levelPositions = levelPositionsNew;
					levelWidth = levelWidthNew;
				}
			}
		}
		
		//add point t 1 (last point)
		pointsBeziered.Add(Vector3.Lerp(tempListPos[tempListPos.Count-2],tempListPos[tempListPos.Count-1],1));
		rotationsBeziered.Add(Quaternion.Lerp(tempListRot[tempListRot.Count-2],tempListRot[tempListRot.Count-1],1));
		widthBeziered.Add(Mathf.Lerp(tempListWidth[tempListWidth.Count-2],tempListWidth[tempListWidth.Count-1],1));

		//bezied track data
		currentPositionsBeziered = pointsBeziered.ToArray ();
		currentRotations = rotationsBeziered.ToArray ();
		currentWidths = widthBeziered.ToArray ();
		
		//save source for update checks
		currentPositionsSource = tempListPos.ToArray ();
		currentRottationsSource = tempListRot.ToArray ();
		currentWidthsSource = tempListWidth.ToArray();
		
		//Debug.Log("pointsCurrent.length: "+pointsCurrent.Length);
		//for (i = 0; i < pointsCurrent.Length; i++) {
		//	Debug.Log(i+":"+pointsCurrent[i]);
		//}
		return true;
	}

	void CreateRoadMeshData(List<Vector3> vertList,List<Vector2> uvList, List<int> triList){
		
		//Vertices
		float[] trackX = new float[trackSettings.widthDetail*2];
		float widthPartSize = 1.0f/(float)trackSettings.widthDetail;
		float currentXPart = -0.5f;
		for(j = 0;j < trackX.Length;j+=2){
			trackX[j] = currentXPart;
			currentXPart+=widthPartSize;
			trackX[j+1] = currentXPart;
		}
		for(i = 0;i < roadLength;i++){
			
			Vector3 pointPos = currentPositionsBeziered[i]- transform.position;
			Vector3 pointPosNex = currentPositionsBeziered[i+1]- transform.position;
			
			Vector3 newVert;
			
			for(j = 0;j < trackX.Length;j+=2){
				
				float roadSizeBezier = currentWidths[i];
				
				newVert = pointPos+(currentRotations[i]*new Vector3(trackX[j]*roadSizeBezier,0,0));
				vertList.Add(newVert);
				
				newVert = pointPos+(currentRotations[i]*new Vector3(trackX[j+1]*roadSizeBezier,0,0));
				vertList.Add(newVert);
				
				roadSizeBezier = currentWidths[i+1];
				
				newVert = pointPosNex+(currentRotations[i+1]*new Vector3(trackX[j]*roadSizeBezier,0,0));
				vertList.Add(newVert);
				
				newVert = pointPosNex+(currentRotations[i+1]*new Vector3(trackX[j+1]*roadSizeBezier,0,0));
				vertList.Add(newVert);
			}
		}
		
		
		//uv
		float tempAferageWidth = 25;/////////////
		float lengthPartSize = ((1f/trackSettings.pointsPerUnityUnit)/tempAferageWidth);
		for(i = 0;i < roadPartLength;i++){
			float texWidthPos = ((i)%trackSettings.widthDetail);
			//float texLengthPos = (i-(i%widthDetail)/widthDetail);
			float texLengthPos = (i/trackSettings.widthDetail);
			
			float luv1 = (texLengthPos*lengthPartSize);
			float luv2 = ((texLengthPos+1)*lengthPartSize);
			
			//0.0f,0.0f
			//1.0f,0.0f
			//0.0f,1.0f
			//1.0f,1.0f
			uvList.Add(new Vector2((widthPartSize*texWidthPos),luv1));
			uvList.Add(new Vector2((widthPartSize*(texWidthPos+1)),luv1));
			uvList.Add(new Vector2((widthPartSize*texWidthPos),luv2));
			uvList.Add(new Vector2((widthPartSize*(texWidthPos+1)),luv2));
		}
		
		//triangles
		for(i = 0;i < roadPartLength;i++){
			//1,0,3,
			//0,2,3
			triList.Add((1+(4*i)));
			triList.Add((0+(4*i)));
			triList.Add((3+(4*i)));
			triList.Add((0+(4*i)));
			triList.Add((2+(4*i)));
			triList.Add((3+(4*i)));
		}
	}

	void CreateWallMeshData(List<Vector3> vertList,List<Vector2> uvList, List<int> triList,TrackSide side){
		
		//Vertices
		for(i = 0;i < roadLength;i++){
			
			Vector3 pointPos = currentPositionsBeziered[i]- transform.position;
			Vector3 pointPosNex = currentPositionsBeziered[i+1]- transform.position;
			
			Vector3 newVert;

			float roadSizeBezier = currentWidths[i];
			Quaternion TempRot = currentRotations[i];
			Quaternion TempRot2 = currentRotations[i+1];
			
			if(side == TrackSide.Right){
				newVert = pointPos+(currentRotations[i]*new Vector3(roadSizeBezier*0.5f,0f,0));
				vertList.Add(newVert);
				
				newVert = pointPos+(currentRotations[i]*new Vector3(roadSizeBezier*0.5f,0,0))+(TempRot*new Vector3(0,trackSettings.wallRightHeight,0));
				vertList.Add(newVert);
				
				roadSizeBezier = currentWidths[i+1];
				
				newVert = pointPosNex+(currentRotations[i+1]*new Vector3(roadSizeBezier*0.5f,0,0));
				vertList.Add(newVert);
				
				newVert = pointPosNex+(currentRotations[i+1]*new Vector3(roadSizeBezier*0.5f,0f,0))+(TempRot2*new Vector3(0,trackSettings.wallRightHeight,0));
				vertList.Add(newVert);
			}else{

				newVert = pointPos+(currentRotations[i]*new Vector3(roadSizeBezier*-0.5f,0,0))+(TempRot*new Vector3(0,trackSettings.WallLefttHeight,0f));
				vertList.Add(newVert);
				
				newVert = pointPos+(currentRotations[i]*new Vector3(roadSizeBezier*-0.5f,0f,0));
				vertList.Add(newVert);
				
				roadSizeBezier = currentWidths[i+1];

				newVert = pointPosNex+(currentRotations[i+1]*new Vector3(roadSizeBezier*-0.5f,0,0))+(TempRot2*new Vector3(0,trackSettings.WallLefttHeight,0f));
				vertList.Add(newVert);
				
				newVert = pointPosNex+(currentRotations[i+1]*new Vector3(roadSizeBezier*-0.5f,0f,0));
				vertList.Add(newVert);
			}
		}
		
		//uvs
		float tempAferageWidth = 25;///////////// work in progress
		float lengthPartSize = ((1f/trackSettings.pointsPerUnityUnit)/tempAferageWidth);
		for(i = 0;i < (roadLength);i++){
			float texWidthPos = ((i)%trackSettings.widthDetail);
			float texLengthPos = (i/trackSettings.widthDetail);
			
			float luv1 = (texLengthPos*lengthPartSize);
			float luv2 = ((texLengthPos+1)*lengthPartSize);
			
			//0.0f,0.0f
			//1.0f,0.0f
			//0.0f,1.0f
			//1.0f,1.0f
			uvList.Add(new Vector2((texWidthPos),luv1));
			uvList.Add(new Vector2(((texWidthPos+1)),luv1));
			uvList.Add(new Vector2((texWidthPos),luv2));
			uvList.Add(new Vector2(((texWidthPos+1)),luv2));
		}
		
		//triangles
		for(i = 0;i < (roadLength);i++){
			//1,0,3,
			//0,2,3
			triList.Add((1+(4*i)));
			triList.Add((0+(4*i)));
			triList.Add((3+(4*i)));
			triList.Add((0+(4*i)));
			triList.Add((2+(4*i)));
			triList.Add((3+(4*i)));
		}
	}
	
	Mesh DataToMesh(string name, Vector3[] vertices, Vector2[] uvs, int[] triangles){
		Mesh newMesh = new Mesh();
		newMesh.name = name;
		newMesh.vertices = vertices;
		newMesh.uv = uvs;
		newMesh.triangles = triangles;
		newMesh.Optimize();
		newMesh.RecalculateNormals();
		return newMesh;
	}

	void CreateMesh(){
	
		// add components if needed
		MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
		if(meshFilter==null){
			meshFilter = gameObject.AddComponent<MeshFilter>();
		}
		meshFilter.sharedMesh = new Mesh();
		MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
		if(meshRenderer==null){
			meshRenderer = gameObject.AddComponent<MeshRenderer>();
		}
		MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
		if(meshCollider==null){
			meshCollider = gameObject.AddComponent<MeshCollider>();
		}
		meshCollider.smoothSphereCollisions = trackSettings.smoothSphereCollision;
		
		// variables needed
		roadLength = currentPositionsBeziered.Length - 1;
		roadPartLength = roadLength*trackSettings.widthDetail;
		int combineSize = 1;
		if(trackSettings.wallRight){
			combineSize++;
		}
		if(trackSettings.wallLeft){
			combineSize++;
		}
		CombineInstance[] combine = new CombineInstance[combineSize];
		int combineNum = 0;

		//main track
		trackVertices = new List<Vector3>();
		trackUvs = new List<Vector2>();
		trackTriangles = new List<int>();
		CreateRoadMeshData(trackVertices,trackUvs,trackTriangles);
		
		tempTrackMesh = DataToMesh("roadMesh", trackVertices.ToArray(), trackUvs.ToArray(), trackTriangles.ToArray());

		combine[combineNum].mesh = tempTrackMesh;
		combine[combineNum].transform = Matrix4x4.identity;
		combineNum++;
		
		//create left wall mesh data and add it to the CombineInstance
		if(trackSettings.wallLeft){
			trackVertices = new List<Vector3>();
			trackUvs = new List<Vector2>();
			trackTriangles = new List<int>();
			CreateWallMeshData(trackVertices,trackUvs,trackTriangles,TrackSide.Left);

			tempTrackMesh = DataToMesh("roadMeshSideLeft", trackVertices.ToArray(), trackUvs.ToArray(), trackTriangles.ToArray());

			combine[combineNum].mesh = tempTrackMesh;
			combine[combineNum].transform = Matrix4x4.identity;
			combineNum++;
		}
		
		//create right wall mesh data and add it to the CombineInstance
		if(trackSettings.wallRight){
			trackVertices = new List<Vector3>();
			trackUvs = new List<Vector2>();
			trackTriangles = new List<int>();
			CreateWallMeshData(trackVertices,trackUvs,trackTriangles,TrackSide.Right);
			
			tempTrackMesh = DataToMesh("roadMeshSideRight", trackVertices.ToArray(), trackUvs.ToArray(), trackTriangles.ToArray());

			combine[combineNum].mesh = tempTrackMesh;
			combine[combineNum].transform = Matrix4x4.identity;
			combineNum++;
		}
		
		//pass the data to the MeshCollider and MeshFilter
		meshFilter.sharedMesh.CombineMeshes(combine);
		meshCollider.sharedMesh.CombineMeshes(combine);
		meshCollider.enabled = false;
		meshCollider.enabled = true;
		
		
		//set material
		meshRenderer.material = trackSettings.trackMaterial;
	}
}


