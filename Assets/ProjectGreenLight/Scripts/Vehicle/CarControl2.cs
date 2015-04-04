using UnityEngine;
using System.Collections;

public class CarControl2 : MonoBehaviour
{

    public float inputMotor = 0f;
    public float inputSteer = 0f;

    [SerializeField]
	private WheelCollider Wheel_FL;
    [SerializeField]
    private WheelCollider Wheel_FR;
    [SerializeField]
    private WheelCollider Wheel_RL;
    [SerializeField]
    private WheelCollider Wheel_RR;

    [SerializeField]
    private float[] GearRatio;
    [SerializeField]
    private int CurrentGear = 0;
    [SerializeField]
    private float EngineTorque = 600f;
    [SerializeField]
    private float MaxEngineRPM = 3000f;
    [SerializeField]
    private float MinEngineRPM = 1000f;
    [SerializeField]
    private float SteerAngle = 10f;
    [SerializeField]
    private Transform COM;
    [SerializeField]
    private float speed_ = 0f;
    [SerializeField]
    private float maxSpeed = 150f;
    [SerializeField]
    private AudioSource skidAudio;
    [SerializeField]
    private float EngineRPM = 0f;

    public float speed
    {
        get
        {
            return speed_;
        }
    }

	void Start () 
	{
		GetComponent<Rigidbody>().centerOfMass = new Vector3(COM.localPosition.x * transform.localScale.x, COM.localPosition.y * transform.localScale.y, COM.localPosition.z * transform.localScale.z);
	}

	void Update () 
	{
		speed_ = GetComponent<Rigidbody>().velocity.magnitude * 3.6f;
		GetComponent<Rigidbody>().drag = GetComponent<Rigidbody>().velocity.magnitude / 100f;
		EngineRPM = (Wheel_FL.rpm + Wheel_FR.rpm) / 2f * GearRatio[CurrentGear];
		
		ShiftGears();

		//Audio
		//audio.pitch = Mathf.Abs(EngineRPM / MaxEngineRPM) + 1.0;
		//if (audio.pitch > 2.0) {
		//	audio.pitch = 2.0;
		//}
		
		//Steering
		Wheel_FL.steerAngle = SteerAngle * inputSteer;
		Wheel_FR.steerAngle = SteerAngle * inputSteer;
		
		//Speed Limiter.
        if (speed_ > maxSpeed)
		{
			Wheel_FL.motorTorque = 0f;
			Wheel_FR.motorTorque = 0f;
		}
		else
		{
			Wheel_FL.motorTorque = EngineTorque / GearRatio[CurrentGear] * inputMotor;
			Wheel_FR.motorTorque = EngineTorque / GearRatio[CurrentGear] * inputMotor;
		}
		
		//Input.
		if(inputMotor <= 0f)
		{
			Wheel_RL.brakeTorque = 30f;
			Wheel_RR.brakeTorque = 30f;
		}
		else if (inputMotor >= 0f)
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
