using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour {
	[SerializeField]
	private float speed = 10.0f;
	[SerializeField]
	private float speedMult = 10.0f;
	[SerializeField]
	private float downForce = 2.0f;
	[SerializeField]
	private float steerPowerBase = 150000f;
	[SerializeField]
	private float steerPowerSec = 30000f;
	[SerializeField]
	private WorldPath path;
	[SerializeField]
	private float gizmoSize = 1f;
	[SerializeField]
	private float lineHight = 5f;

	private Rigidbody rigidbody;
	private float input = 0f;
	private float steer = 0f;

	[SerializeField]
	private Transform test;

	[SerializeField]
	private ShipTruster[] thrusters;

	[SerializeField]
	private Transform rotatePointBack;
	[SerializeField]
	private Transform rotatePointFront;
	[SerializeField]
	private float frontRotatePowerMult = 0.2f;

	private Vector3[] trustDirs;

	void OnDrawGizmos(){
		Node pathPoint = path.GetClosestPoint (transform.position);

		//draw line up and down from closest path point
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere (pathPoint.center.position, gizmoSize);

		//draw line up from closest path point
		Gizmos.color = Color.magenta;
		Vector3 closestPoint = pathPoint.center.position;
		Quaternion rot = pathPoint.center.rotation;
		Vector3 closestPointUp = (closestPoint+ (rot*Vector3.up*5));
		Gizmos.DrawLine(closestPoint,closestPointUp);
		//draw line down from closest path point
		Vector3 closestPointDown = (closestPoint+ (rot*Vector3.down*5));
		Gizmos.DrawLine(closestPoint,closestPointDown);
		//draw line left from closest path point
		Vector3 closestPointLeft = (closestPoint+ (rot*Vector3.left*5));
		Gizmos.DrawLine(closestPoint,closestPointLeft);
		//draw line right from closest path point
		Vector3 closestPointRight = (closestPoint+ (rot*Vector3.right*5));
		Gizmos.DrawLine(closestPoint,closestPointRight);

		//draw force forwared
		Vector3 forceForwared = (transform.position+(transform.rotation*(Vector3.forward *5f)));
		Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.position,forceForwared);

		//draw force down
		Vector3 forceDown = (transform.position+ (rot*Vector3.down* 5f));
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(transform.position,forceDown);

		//flyght line
		Vector3 thisLine = pathPoint.center.position;
		Vector3 nextLinePos = pathPoint.next.center.position;
		Vector3 previousLinePos = pathPoint.previous.center.position;

		//draw flyght aim line
		Gizmos.color = Color.white;
		Gizmos.DrawLine(previousLinePos,thisLine);
		Gizmos.DrawLine(thisLine,nextLinePos);
		
		thisLine = thisLine + (pathPoint.center.rotation * Vector3.up * lineHight);
		nextLinePos = nextLinePos + (pathPoint.next.center.rotation * Vector3.up * lineHight);
		previousLinePos = previousLinePos + (pathPoint.previous.center.rotation * Vector3.up * lineHight);

		//draw flyght aim line 2
		Gizmos.color = Color.white;
		Gizmos.DrawLine(previousLinePos,thisLine);
		Gizmos.DrawLine(thisLine,nextLinePos);


		//estimate truster calc pos
		Vector3 shipZDist = DrawVectors(thisLine,transform.position,rot,false);

		//shipZDist = ((pathPoint.center.rotation*(transform.position-thisLine))+thisLine);
		//Vector3 shipZLeft = ((pathPoint.center.rotation*(transform.position-thisLine+(Vector3.left*5f)))+thisLine);
		//Vector3 shipZRight = ((pathPoint.center.rotation*(transform.position-thisLine+(Vector3.right*5f)))+thisLine);
		//Vector3 shipZFornt = ((pathPoint.center.rotation*(transform.position-thisLine+(Vector3.forward*5f)))+thisLine);
		//Vector3 shipZBack = ((pathPoint.center.rotation*(transform.position-thisLine+(Vector3.back*5f)))+thisLine);

		//Gizmos.DrawSphere ((shipZDist), 2f);
		//Gizmos.DrawLine(shipZLeft,shipZRight);
		//Gizmos.DrawLine(shipZFornt,shipZBack);

		////////////////////

		//rot = Quaternion.Lerp( pathPoint.next.center.rotation , pathPoint.center.rotation,0.5f);

		Quaternion rotOther;
		float otherzdelta;
		Vector3 otherLinePos;
		if (shipZDist.z > 0) {
			rotOther = pathPoint.next.center.rotation;
			otherLinePos = nextLinePos;
		} else {
			rotOther = pathPoint.previous.center.rotation;
			otherLinePos = previousLinePos;
		}
		Vector3 shipZDistOther = (Quaternion.Inverse( rotOther)*(transform.position-otherLinePos));
		float zdeltaOther = shipZDistOther.z;
		float zdeltaTotal =  Mathf.Abs(shipZDist.z)+Mathf.Abs( zdeltaOther);
		float zLerp;

		Vector3 finalLinePos;
		Quaternion finalLineRot;
		if (shipZDist.z > 0) {
			zLerp = (shipZDist.z/zdeltaTotal);
			finalLinePos =Vector3.Lerp(thisLine,nextLinePos,zLerp);
			finalLineRot =Quaternion.Lerp(pathPoint.center.rotation,pathPoint.next.center.rotation,zLerp);
		}else{
			zLerp = (zdeltaOther/zdeltaTotal);
			finalLinePos =Vector3.Lerp(previousLinePos,thisLine,zLerp);
			finalLineRot =Quaternion.Lerp(pathPoint.previous.center.rotation,pathPoint.center.rotation,zLerp);
		}
		DrawVectors (finalLinePos,transform.position, finalLineRot,true);
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere (finalLinePos,3f);

		//draw force points
		trustDirs = new Vector3[4];
		for (int i = 0; i < thrusters.Length; i++) {
			Vector3 pointBall = GetDrawVectors(i, finalLinePos, thrusters[i].transform.position, finalLineRot,true);
		}

		//Debug.Log ("zLerp:"+zLerp+"\nzdelta:"+shipZDist.z+" - zdeltaOther:"+zdeltaOther);
	}

	void CalcTrustersVectors(){
		Node pathPoint = path.GetClosestPoint (transform.position);
		Quaternion rot = pathPoint.center.rotation;
		
		//move text object
		test.position = pathPoint.center.position;
		test.rotation = pathPoint.center.rotation;
		
		//flyght line
		Vector3 thisLine = pathPoint.center.position;
		Vector3 nextLinePos = pathPoint.next.center.position;
		Vector3 previousLinePos = pathPoint.previous.center.position;
		
		thisLine = thisLine + (pathPoint.center.rotation * Vector3.up * lineHight);
		nextLinePos = nextLinePos + (pathPoint.next.center.rotation * Vector3.up * lineHight);
		previousLinePos = previousLinePos + (pathPoint.previous.center.rotation * Vector3.up * lineHight);

		//estimate truster calc pos
		Vector3 shipZDist = DrawVectors(thisLine,transform.position,rot,false);
		
		Quaternion rotOther;
		float otherzdelta;
		Vector3 otherLinePos;
		if (shipZDist.z > 0) {
			rotOther = pathPoint.next.center.rotation;
			otherLinePos = nextLinePos;
		} else {
			rotOther = pathPoint.previous.center.rotation;
			otherLinePos = previousLinePos;
		}
		Vector3 shipZDistOther = (Quaternion.Inverse( rotOther)*(transform.position-otherLinePos));
		float zdeltaOther = shipZDistOther.z;
		float zdeltaTotal =  Mathf.Abs(shipZDist.z)+Mathf.Abs( zdeltaOther);
		float zLerp;
		
		Vector3 finalLinePos;
		Quaternion finalLineRot;
		if (shipZDist.z > 0) {
			zLerp = (shipZDist.z/zdeltaTotal);
			finalLinePos =Vector3.Lerp(thisLine,nextLinePos,zLerp);
			finalLineRot =Quaternion.Lerp(pathPoint.center.rotation,pathPoint.next.center.rotation,zLerp);
		}else{
			zLerp = (zdeltaOther/zdeltaTotal);
			finalLinePos =Vector3.Lerp(previousLinePos,thisLine,zLerp);
			finalLineRot =Quaternion.Lerp(pathPoint.previous.center.rotation,pathPoint.center.rotation,zLerp);
		}
		
		//draw force points
		trustDirs = new Vector3[4];
		for (int i = 0; i < thrusters.Length; i++) {
			Vector3 pointBall = GetDrawVectors(i, finalLinePos, thrusters[i].transform.position, finalLineRot,false);
		}
	}

	Vector3 GetDrawVectors(int i,Vector3 fromPos, Vector3 toPos, Quaternion fromRotate, bool draw){
		Vector3 vectors = (Quaternion.Inverse( fromRotate)*(toPos-fromPos));
		
		float xdelta = vectors.x;
		float ydelta = vectors.y;
		float zdelta = vectors.z;

		Vector3 XdeltaVector = (fromRotate * new Vector3 (xdelta, 0, 0));
		Vector3 YdeltaVector = (fromRotate * new Vector3 (0, ydelta, 0));
		Vector3 ZdeltaVector = (fromRotate * new Vector3 (0, 0, zdelta));
		
		Vector3 deltaDrawPoint1 = fromPos;
		Vector3 deltaDrawPoint2 = (fromPos + ZdeltaVector);
		Vector3 deltaDrawPoint3 = (fromPos + ZdeltaVector + XdeltaVector);
		Vector3 deltaDrawPoint4 = (fromPos + ZdeltaVector + XdeltaVector + YdeltaVector);
		
		if(!draw){
			trustDirs[i] = deltaDrawPoint3;
		}
		//draw vector components
		if (draw) {
			Gizmos.color = Color.red;
			Gizmos.DrawLine (deltaDrawPoint1, deltaDrawPoint2);
			Gizmos.color = Color.blue;
			Gizmos.DrawLine (deltaDrawPoint2, deltaDrawPoint3);
			Gizmos.color = Color.cyan;
			Gizmos.DrawLine (deltaDrawPoint3, deltaDrawPoint4);
		}
		return vectors;
	}

	Vector3 DrawVectors(Vector3 fromPos, Vector3 toPos, Quaternion fromRotate, bool draw){
		Vector3 vectors = (Quaternion.Inverse( fromRotate)*(toPos-fromPos));
		
		float xdelta = vectors.x;
		float ydelta = vectors.y;
		float zdelta = vectors.z;
		
		//draw vector components
		if (draw) {
			Vector3 XdeltaVector = (fromRotate * new Vector3 (xdelta, 0, 0));
			Vector3 YdeltaVector = (fromRotate * new Vector3 (0, ydelta, 0));
			Vector3 ZdeltaVector = (fromRotate * new Vector3 (0, 0, zdelta));


			Vector3 deltaDrawPoint1 = fromPos;
			Vector3 deltaDrawPoint2 = (fromPos + ZdeltaVector);
			Vector3 deltaDrawPoint3 = (fromPos + ZdeltaVector + XdeltaVector);
			Vector3 deltaDrawPoint4 = (fromPos + ZdeltaVector + XdeltaVector + YdeltaVector);
			Gizmos.color = Color.red;
			Gizmos.DrawLine (deltaDrawPoint1, deltaDrawPoint2);
			Gizmos.color = Color.blue;
			Gizmos.DrawLine (deltaDrawPoint2, deltaDrawPoint3);
			Gizmos.color = Color.cyan;
			Gizmos.DrawLine (deltaDrawPoint3, deltaDrawPoint4);
		}
		return vectors;
	}


	float ShipTargetHeightPos(){
		Node pathPoint = path.GetClosestPoint (transform.position);
		//pathPoint.center.
		return 0;
	}

	void Start () {
		rigidbody = GetComponent<Rigidbody> ();
	}

	void FixedUpdate (){
		Node colsestPatPoint = path.GetClosestPoint (transform.position);

		//force forwared
		Vector3 forceForwared = (transform.rotation*(Vector3.forward * input * speed *speedMult));
		rigidbody.AddForce (forceForwared);

		//force down
		Vector3 forceDown = colsestPatPoint.center.rotation*Vector3.down*downForce;
		rigidbody.AddForce (forceDown);

		//rotate
		//rigidbody.AddRelativeTorque ((Vector3.up*steer*steerPower*rigidbody.velocity.magnitude));
		rigidbody.AddForceAtPosition (transform.rotation*((Vector3.right*steer*(steerPowerBase+(steerPowerSec*rigidbody.velocity.magnitude)))),rotatePointFront.position);

		rigidbody.AddForceAtPosition (transform.rotation*((Vector3.right*steer*(frontRotatePowerMult*(steerPowerBase+(steerPowerSec*rigidbody.velocity.magnitude))))),rotatePointBack.position);

		CalcTrustersVectors ();
		Trusters ();
		//Debug.Log ("rigidbody.velocity.magnitude:  --"+rigidbody.velocity.magnitude);


		input = 0f;
		steer = 0f;
	}

	void Trusters(){
		if (trustDirs != null) {
			for (int i = 0; i < thrusters.Length; i++) {
				float dist = Vector3.Distance (thrusters [i].transform.position, trustDirs [i]);
				Vector3 vectDir = (thrusters [i].transform.position - trustDirs [i]);
				Debug.Log("vectDir:"+vectDir);
				Debug.DrawRay(thrusters [i].transform.position,vectDir,Color.yellow);
				thrusters [i].Direction = vectDir;

				Vector3 thrusterdir = (thrusters [i].Force * thrusters [i].Direction);
				//Debug.Log("thrusterdir:"+thrusterdir);
				//rigidbody.AddForceAtPosition ((dist*thrusters[i].Force*vectDir),thrusters[i].transform.position);
				rigidbody.AddForceAtPosition (thrusterdir, thrusters [i].transform.position);
			}
		}
	}

	void Update () {
		if(Input.GetKey(KeyCode.W)){
			input += Time.deltaTime;
		}
		if(Input.GetKey(KeyCode.S)){
			input -= Time.deltaTime;
		}
		if(Input.GetKey(KeyCode.D)){
			steer += Time.deltaTime;
		}
		if(Input.GetKey(KeyCode.A)){
			steer -= Time.deltaTime;
		}
	}
}
