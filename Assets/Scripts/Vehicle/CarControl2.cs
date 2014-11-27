using UnityEngine;
using System.Collections;

public class CarControl2 : MonoBehaviour {

	public WheelCollider Wheel_FL;
	public WheelCollider Wheel_FR;
	public WheelCollider Wheel_RL;
	public WheelCollider Wheel_RR;

	public float[] GearRatio;
	public int CurrentGear = 0;
	public float EngineTorque = 600f;
	public float MaxEngineRPM = 3000f;
	public float MinEngineRPM = 1000f;
	public float SteerAngle = 10f;
	public Transform COM;
	public float Speed = 0f;
	public float maxSpeed = 150f;
	public AudioSource skidAudio;
	private float EngineRPM = 0f;
	private float motorInput = 0f;

	void Start () 
	{
		rigidbody.centerOfMass = new Vector3(COM.localPosition.x * transform.localScale.x, COM.localPosition.y * transform.localScale.y, COM.localPosition.z * transform.localScale.z);
	}

	void Update () 
	{
		Speed = rigidbody.velocity.magnitude * 3.6f;
		rigidbody.drag = rigidbody.velocity.magnitude / 100f;
		EngineRPM = (Wheel_FL.rpm + Wheel_FR.rpm) / 2f * GearRatio[CurrentGear];
		
		ShiftGears();
		
		//Input For MotorInput.
		motorInput = Input.GetAxis("Vertical");
		
		//Audio
		//audio.pitch = Mathf.Abs(EngineRPM / MaxEngineRPM) + 1.0;
		//if (audio.pitch > 2.0) {
		//	audio.pitch = 2.0;
		//}
		
		//Steering
		Wheel_FL.steerAngle = SteerAngle * Input.GetAxis("Horizontal");
		Wheel_FR.steerAngle = SteerAngle * Input.GetAxis("Horizontal");
		
		//Speed Limiter.
		if(Speed > maxSpeed)
		{
			Wheel_FL.motorTorque = 0f;
			Wheel_FR.motorTorque = 0f;
		}
		else
		{
			Wheel_FL.motorTorque = EngineTorque / GearRatio[CurrentGear] * Input.GetAxis("Vertical");
			Wheel_FR.motorTorque = EngineTorque / GearRatio[CurrentGear] * Input.GetAxis("Vertical");
		}
		
		//Input.
		if(motorInput <= 0f)
		{
			Wheel_RL.brakeTorque = 30f;
			Wheel_RR.brakeTorque = 30f;
		}
		else if (motorInput >= 0f)
		{
			Wheel_RL.brakeTorque = 0f;
			Wheel_RR.brakeTorque = 0f;
		}
		
		//SkidAudio.
		/*var CorrespondingGroundHit : WheelHit;
		Wheel_RR.GetGroundHit( CorrespondingGroundHit );
		if(Mathf.Abs(CorrespondingGroundHit.sidewaysSlip) > 10) {
			skidAudio.enabled = true;
		}else{
			skidAudio.enabled = false;
		}*/
			
		//HandBrake
		if(Input.GetButtonDown("Jump"))
		{
			Wheel_FL.brakeTorque = 100f;
			Wheel_FR.brakeTorque = 100f;
		}
		if(Input.GetButtonUp("Jump"))
		{
			Wheel_FL.brakeTorque = 0f;
			Wheel_FR.brakeTorque = 0f;
		}
	}

	private void ShiftGears() 
	{
		if (EngineRPM >= MaxEngineRPM) 
		{
			int AppropriateGear = CurrentGear;
			
			for ( int i = 0; i < GearRatio.Length; i ++) 
			{
				if(Wheel_FL.rpm * GearRatio[i] < MaxEngineRPM) 
				{
					AppropriateGear = i;
					break;
				}
			}
			CurrentGear = AppropriateGear;
		}
		
		if(EngineRPM <= MinEngineRPM) 
		{
			int AppropriateGear = CurrentGear;
			for ( int j = GearRatio.Length - 1; j >= 0; j -- ) 
			{
				if ( Wheel_FL.rpm * GearRatio[j] > MinEngineRPM ) 
				{
					AppropriateGear = j;
					break;
				}
			}
			CurrentGear = AppropriateGear;
		}
	}
}
