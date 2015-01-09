using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour 
{
    [SerializeField]
	private Transform target_;
	private Transform cam;

	private float posX = 0f;
	private float posY = 0f;

	private float height = 2f;
	private float distance = 6f;

    public Transform target
    {
        set
        {
            target_ = value;
            Init();
        }
    }

	void Init () 
	{
        cam = target_;

		Vector3 a = transform.eulerAngles;
		posX = a.x;
		posY = a.y;

		if(this.rigidbody != null)
		{
			rigidbody.freezeRotation = true;
		}
	}

	void LateUpdate () 
	{
        if (target_ != null)
        {
            float tA = target_.eulerAngles.y;
            float cA = cam.eulerAngles.y;
            posX = Mathf.LerpAngle(cA, tA, 3f * Time.deltaTime);

            Quaternion rot = Quaternion.Euler(posX, posY, 0);
            Vector3 targetMod = new Vector3(0, height, 0);

            Vector3 pos = target_.position - (rot * Vector3.forward * (distance) + targetMod);
            //Vector3 pos2 = target.position - (rot * Vector3.forward * (0.1) + targetMod);

            cam.rotation = rot;
            cam.position = pos;
        }
	}
}
