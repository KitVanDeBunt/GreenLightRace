// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden

using UnityEngine;
using System.Collections;

public class VehicleFollow : MonoBehaviour
{
    public Transform target;
    [SerializeField]
    private bool fixAngle;

    [SerializeField]
    private bool prevButtonRight = false;

    private Transform myTransform;

    [SerializeField]
    private float targetHeight = 2.0f;
    [SerializeField]
    private float targetRight = 0.0f;
    [SerializeField]
    private float distance = 6.0f;

    [SerializeField]
    private float maxDistance = 20;
    [SerializeField]
    private float minDistance = 5;

    [SerializeField]
    private float xSpeed = 250.0f;
    [SerializeField]
    private float ySpeed = 120.0f;

    [SerializeField]
    private float yMinLimit = -20;
    [SerializeField]
    private float yMaxLimit = 80;

    [SerializeField]
    private float zoomRate = 1;

    [SerializeField]
    private float rotationDampening = 3.0f;

    //[SerializeField]
    //private float theta2 = 0.5f;

    private float x = 0.0f;
    private float y = 0.0f;

    /*
    private Vector3 fwd = new Vector3();
    private Vector3 rightVector = new Vector3();
    private Vector3 upVector = new Vector3();
    private Vector3 movingVector = new Vector3();
    private Vector3 collisionVector = new Vector3();
    private bool isColliding = false;
    */
    private float distmod = 0.0f;

    void Start()
    {
        myTransform = transform;
        Vector3 angles = myTransform.eulerAngles;
        x = angles.y;
        y = angles.x;

        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;
    }

    void LateUpdate()
    {
        if (!target)
            return;

        //if (Input.GetMouseButtonUp(0)) prevButtonRight = false;
        //if (Input.GetMouseButtonUp(1)) prevButtonRight = true;

        // If either mouse buttons are down, let them govern camera position
        if (Input.GetMouseButton(0))
        {
            x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            // otherwise, ease behind the target if any of the directional keys are pressed
        }
        else if (prevButtonRight)
        {
            float targetRotationAngle = target.eulerAngles.y;
            float currentRotationAngle = myTransform.eulerAngles.y;
            x = Mathf.LerpAngle(currentRotationAngle, targetRotationAngle, rotationDampening * Time.deltaTime);
        }

        distance -= Input.GetAxis("Mouse ScrollWheel") * zoomRate * Mathf.Abs(distance);// * Time.deltaTime
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        y = ClampAngle(y, yMinLimit, yMaxLimit);

        Quaternion rotation = Quaternion.Euler(y, x, 0);
        if (fixAngle && prevButtonRight)
        {
            //traget rotation
            rotation = Quaternion.Lerp(rotation, target.rotation, 0.5f);
            //current rotation
            rotation = Quaternion.Lerp(myTransform.rotation, rotation, rotationDampening * Time.deltaTime);
        }
        Vector3 targetMod = new Vector3(0, -targetHeight, 0) - (rotation * Vector3.right * targetRight);
        int layerMask = 1 << 8;
        layerMask = ~layerMask;
        Vector3 position = target.position - (rotation * Vector3.forward * (distance - distmod) + targetMod);
        Vector3 position2 = target.position - (rotation * Vector3.forward * (0.1f) + targetMod);
        //////////////
         // Check to see if we have a collision
         if ((Physics.CheckSphere (position, 0.4f, layerMask)||Physics.Linecast (position2, position, layerMask))&&(distmod<distance))
         {
            position = target.position - (rotation * Vector3.forward * (distance-distmod) + new Vector3(0,-targetHeight,0));
            distmod=Mathf.Lerp(distmod,distance,Time.deltaTime*2);
         }
         else
         {
            float newdistmod=Mathf.Lerp(distmod,0.0f,Time.deltaTime*2);
            if (newdistmod<0.1f) newdistmod=0.0f;
            if (!Physics.CheckSphere (target.position - (rotation * Vector3.forward * (distance-newdistmod) + targetMod), 0.4f, layerMask)&&!Physics.Linecast (position2, target.position - (rotation * Vector3.forward * (distance-newdistmod) + targetMod), layerMask)&&(distmod!=0.0f)){
               distmod=newdistmod;
            }
         }
       ////////////
        position = Vector3.Slerp(transform.position, position, Time.deltaTime * 100);   

        myTransform.rotation = rotation;
        myTransform.position = position;
    }

    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

}