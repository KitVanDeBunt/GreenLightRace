using UnityEngine;
using System.Collections;

public class WheelAlignment2 : MonoBehaviour {

	public WheelCollider CorrespondingCollider;
	public GameObject SlipPrefab;
	private float RotationValue = 0f;

	void Update () 
	{
		RaycastHit hit;
		Vector3 ColliderCenterPoint = CorrespondingCollider.transform.TransformPoint( CorrespondingCollider.center );
		
		if(Physics.Raycast( ColliderCenterPoint, -CorrespondingCollider.transform.up, out hit, CorrespondingCollider.suspensionDistance + CorrespondingCollider.radius)) 
		{
			transform.position = hit.point + (CorrespondingCollider.transform.up * CorrespondingCollider.radius);
		}
		else
		{
			transform.position = ColliderCenterPoint - (CorrespondingCollider.transform.up * CorrespondingCollider.suspensionDistance);
		}
		
		transform.rotation = CorrespondingCollider.transform.rotation * Quaternion.Euler( 0, CorrespondingCollider.steerAngle + 90, RotationValue );
		RotationValue += CorrespondingCollider.rpm * ( 360/60 ) * Time.deltaTime;
		
		WheelHit CorrespondingGroundHit;
		CorrespondingCollider.GetGroundHit( out CorrespondingGroundHit );
		
		/*if ( Mathf.Abs( CorrespondingGroundHit.sidewaysSlip ) > 10 ) 
		{
			if ( SlipPrefab ) 
			{
				Instantiate( SlipPrefab, CorrespondingGroundHit.point, Quaternion.identity );
			}
		}*/
	}
}
