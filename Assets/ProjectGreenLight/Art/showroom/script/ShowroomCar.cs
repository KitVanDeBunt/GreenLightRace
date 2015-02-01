using UnityEngine;
using System.Collections;

public class ShowroomCar : MonoBehaviour 
{
    [SerializeField]
	private CarID carId_;

	public CarID carId
	{
        get
        {
            return carId_;
        }
	}
}
