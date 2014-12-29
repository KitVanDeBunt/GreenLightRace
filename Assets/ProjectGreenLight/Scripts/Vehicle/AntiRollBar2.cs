using UnityEngine;
using System.Collections;

public class AntiRollBar2 : MonoBehaviour {

	public WheelCollider WheelL;
	public WheelCollider WheelR;
	public float AntiRoll = 5000f;

	void FixedUpdate () 
	{
		WheelHit hit = new WheelHit();
		float travelL = 1f;
		float travelR = 1f;
		bool groundedL = WheelL.GetGroundHit(out hit);
		
		if (groundedL) 
		{
			travelL = (-WheelL.transform.InverseTransformPoint (hit.point).y - WheelL.radius) / WheelL.suspensionDistance;
		}

		bool groundedR = WheelR.GetGroundHit(out hit);
		if (groundedR) 
		{
			travelR = (-WheelR.transform.InverseTransformPoint (hit.point).y - WheelR.radius) / WheelR.suspensionDistance;
		}

		float antiRollForce = (travelL - travelR) * AntiRoll;
		if (groundedL) 
		{
			rigidbody.AddForceAtPosition (WheelL.transform.up * -antiRollForce, WheelL.transform.position); 
		}
		if (groundedR) 
		{
			rigidbody.AddForceAtPosition (WheelR.transform.up * antiRollForce, WheelR.transform.position); 
		}
	}
}
