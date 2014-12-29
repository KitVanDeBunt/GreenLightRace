var target : Transform;
private var myTransform:Transform;

var targetHeight : float = 2.0;
var targetRight : float = 0.0;
var distance : float = 6.0;

var prevButtonRight:boolean=false;

var maxDistance : float = 20;
var minDistance : float = 5;

var xSpeed : float = 250.0;
var ySpeed : float = 120.0;

var yMinLimit : float = -20;
var yMaxLimit : float = 80;

var zoomRate : float = 1;

var rotationDampening : float = 3.0;

var theta2 : float = 0.5;

private var x : float = 0.0;
private var y : float = 0.0;

private var fwd :Vector3= new Vector3();
private var rightVector :Vector3= new Vector3();
private var upVector :Vector3= new Vector3();
private var movingVector :Vector3= new Vector3();
private var collisionVector :Vector3= new Vector3();
private var isColliding : boolean = false;

private var distmod : float = 0.0;

function Start () {
   myTransform = transform;
    var angles :Vector3= myTransform.eulerAngles;
    x = angles.y;
    y = angles.x;

   // Make the rigid body not change rotation
      if (rigidbody)
      rigidbody.freezeRotation = true;
}

function LateUpdate () {
   if(!target)
      return;
      
   if (Input.GetMouseButtonUp(0)) prevButtonRight=false;
   if (Input.GetMouseButtonUp(1)) prevButtonRight=true;
   
   // If either mouse buttons are down, let them govern camera position
   if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
   {
   x += Input.GetAxis("Mouse X") * xSpeed * 0.02;
   y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02;
   
   // otherwise, ease behind the target if any of the directional keys are pressed
   } else if(prevButtonRight) {
      var targetRotationAngle = target.eulerAngles.y;
      var currentRotationAngle = myTransform.eulerAngles.y;
      x = Mathf.LerpAngle(currentRotationAngle, targetRotationAngle, rotationDampening * Time.deltaTime);
   }
   
   distance -= Input.GetAxis("Mouse ScrollWheel") * zoomRate * Mathf.Abs(distance);// * Time.deltaTime
   distance = Mathf.Clamp(distance, minDistance, maxDistance);
   
   y = ClampAngle(y, yMinLimit, yMaxLimit);
   
   var rotation:Quaternion = Quaternion.Euler(y, x, 0);
   var targetMod : Vector3=Vector3(0,-targetHeight,0) - (rotation*Vector3.right*targetRight);
   var layerMask = 1<<8;
   layerMask = ~layerMask;
   var position = target.position - (rotation * Vector3.forward * (distance-distmod) + targetMod);
   var position2= target.position - (rotation * Vector3.forward * (0.1) + targetMod);
  /* 
   // Check to see if we have a collision
   if ((Physics.CheckSphere (position, 0.4, layerMask)||Physics.Linecast (position2, position, layerMask))&&(distmod<distance))
   {
      position = target.position - (rotation * Vector3.forward * (distance-distmod) + Vector3(0,-targetHeight,0));
      distmod=Mathf.Lerp(distmod,distance,Time.deltaTime*2);
   }
   else
   {
      var newdistmod=Mathf.Lerp(distmod,0.0,Time.deltaTime*2);
      if (newdistmod<0.1) newdistmod=0.0;
      if (!Physics.CheckSphere (target.position - (rotation * Vector3.forward * (distance-newdistmod) + targetMod), .4, layerMask)&&!Physics.Linecast (position2, target.position - (rotation * Vector3.forward * (distance-newdistmod) + targetMod), layerMask)&&(distmod!=0.0)){
         distmod=newdistmod;
      }
   }
 */  
   //position = Vector3.Slerp(transform.position, position, Time.deltaTime * 100);   
   
   myTransform.rotation = rotation;
   myTransform.position = position;
}

static function ClampAngle (angle : float, min : float, max : float) {
   if (angle < -360)
      angle += 360;
   if (angle > 360)
      angle -= 360;
   return Mathf.Clamp (angle, min, max);
}
