using UnityEngine;
using System.Collections;

public class CarPlayer : MonoBehaviour {

    private CarControl2 control;

    void Start()
    {
        control = gameObject.GetComponent<CarControl2>();
    }
	void Update () 
    {
        control.inputMotor = Input.GetAxis("Vertical");
        control.inputSteer = Input.GetAxis("Horizontal");
	}
}
