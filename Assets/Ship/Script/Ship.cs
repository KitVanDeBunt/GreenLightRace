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

	private Rigidbody rigidB;
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

	[Tooltip("max speed in km/h")]
	[SerializeField]
	private float maxSpeed = 500f; 

	//boosters
	[SerializeField]
	private ParticleSystem[] boosters;
	[SerializeField]
	private float boosterParticleMultiplyer = 2f;

	private float boosterSpeed = 0f; 

	private float deltaTimeForce;

	// ui
	[SerializeField]
	private UnityEngine.UI.Text speedText;
	void UpdateUI(){
		string speedString = ((int)(rigidB.velocity.magnitude*3.6f)).ToString();
		while (speedString.Length < 6) {
			speedString = ("0"+speedString);
		}
		speedString+="km/h";
		
		//Debug.Log (speedString.Length);
		speedText.text = speedString;
	}


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
		rigidB = GetComponent<Rigidbody> ();
	}

	void FixedUpdate (){
		Node colsestPatPoint = path.GetClosestPoint (transform.position);

		//// v3
		Vector3 shipVelocityVector = (rigidB.mass * 50f * rigidB.velocity * -1) * ((rigidB.velocity.magnitude * 3.6f) / (16000f * 3.6f));
		Vector3 ShipVelocityVectorNormalized = shipVelocityVector.normalized;
		Vector3 drawShipDragVector = ShipVelocityVectorNormalized*10f;
		Vector3 shipAngleVector = transform.rotation * Vector3.back;
		Vector3 drawShipAngleVector = shipAngleVector*10f;

		Debug.DrawRay (transform.position, drawShipAngleVector, Color.red);
		Debug.DrawRay (transform.position, drawShipDragVector , Color.white);

		float ShipVelocityAngleDiffrence = Vector3.Angle (shipAngleVector, shipVelocityVector);
		float dragSteerPower1 = (ShipVelocityAngleDiffrence / 180f);
		float dragSteerPower2 = Mathf.Clamp((ShipVelocityAngleDiffrence / 45f),0f,1f);

		//dragSteerPower1 = 1f;		
		//dragSteerPower2 = 0f;

		//Debug.Log ("dragSteerPower1:"+dragSteerPower1+"\ndragSteerPower2:"+dragSteerPower2);
		//float shipAngle = drawNorm*
		//rigidB.AddForce (drag);

		//force forwared / drag
		Vector3 forceForwared = ((transform.rotation*(Vector3.forward * input * speed *speedMult)));
		rigidB.AddForce ((forceForwared*deltaTimeForce));
		//drag
		Vector3 dragF = (rigidB.mass * 50f * ((rigidB.velocity*(1-dragSteerPower1))+(shipAngleVector*dragSteerPower2*rigidB.velocity.magnitude)) * -1) * (Mathf.Clamp (((rigidB.velocity.magnitude * 3.6f) / maxSpeed), 0.0f, 1.0f));
		rigidB.AddForce ((dragF*deltaTimeForce));

		/*
		Vector3 forceForwared = (transform.rotation*(Vector3.forward * input * speed *speedMult));
		float forceDrag = (1f - ((rigidB.velocity.magnitude * 3.6f) / (maxSpeed * 3.6f)));
		rigidB.AddForce ((forceForwared*forceDrag));*/

		//force down
		Vector3 forceDown = colsestPatPoint.center.rotation*Vector3.down*downForce;
		rigidB.AddForce ((forceDown*deltaTimeForce));

		//rotate
		//// v1
		//rigidB.AddRelativeTorque ((Vector3.up*steer*steerPower*rigidB.velocity.magnitude));
		//// v2
		rigidB.AddForceAtPosition ((transform.rotation*((Vector3.right*steer*(steerPowerBase+(steerPowerSec*rigidB.velocity.magnitude))))*deltaTimeForce),rotatePointFront.position);
		//
		rigidB.AddForceAtPosition ((transform.rotation*((Vector3.right*steer*(frontRotatePowerMult*(steerPowerBase+(steerPowerSec*rigidB.velocity.magnitude)))))*deltaTimeForce),rotatePointBack.position);


		CalcTrustersVectors ();
		Trusters ();
		//Debug.Log ("rigidB.velocity.magnitude:  --"+rigidB.velocity.magnitude);


		input = 0f;
		steer = 0f;
	}

	void Trusters(){
		if (trustDirs != null) {
			for (int i = 0; i < thrusters.Length; i++) {
				float dist = Vector3.Distance (thrusters [i].transform.position, trustDirs [i]);
				Vector3 vectDir = (thrusters [i].transform.position - trustDirs [i]);
				//Debug.Log("vectDir:"+vectDir);
				Debug.DrawRay(thrusters [i].transform.position,vectDir,Color.yellow);
				thrusters [i].Direction = vectDir;

				Vector3 thrusterdir = (thrusters [i].Force * thrusters [i].Direction);
				//Debug.Log("thrusterdir:"+thrusterdir);
				//rigidB.AddForceAtPosition ((dist*thrusters[i].Force*vectDir),thrusters[i].transform.position);
				rigidB.AddForceAtPosition ((thrusterdir*deltaTimeForce), thrusters [i].transform.position);
			}
		}
	}

	
	void UpdateBoosters(){
		if (input > 0 && boosterSpeed < 1f) {
			boosterSpeed += input;
			boosterSpeed += Time.deltaTime;
		} else if(boosterSpeed > 0){
			boosterSpeed -= Time.deltaTime;
		}
		foreach (var booster in boosters) {
			booster.startSpeed = (25f+(100f*boosterSpeed)); 
			booster.emissionRate = ((boosterSpeed*boosterParticleMultiplyer*10f)+20f);
		}
	}

	void Update () {
		deltaTimeForce = 80f * Time.deltaTime;
		UpdateUI ();
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
		UpdateBoosters();
	}
}
