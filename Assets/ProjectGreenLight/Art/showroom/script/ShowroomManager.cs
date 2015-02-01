using UnityEngine;
using System.Collections;

public class ShowroomManager : MonoBehaviour 
{
	public ShowroomCar[] cars;
	public int carSelected;
	public int carLength;

	public AudioSource carSoundStart;
	public AudioSource carSoundIdle;
	private float idleSoundDelay;
	private bool carIdle = true;

	void Start () 
	{
	}

	public void init()
	{		
		carSelected = 0;	
		
		cars = GetComponentsInChildren<ShowroomCar>();
		carLength = cars.Length;
		
		for(int i = 0; i < carLength; i++)
		{
			cars[i].gameObject.SetActive(false);
		}
	}

	public int switchCar(int dir)
	{
		stopSound();
		idleSoundDelay = 2.5f;
		carIdle = false;
		carSoundStart.Play();

		cars[carSelected].gameObject.SetActive(false);
		if(carSelected + dir < 0)
		{
			carSelected = carLength - 1;
		}
		else if(carSelected + dir >= carLength)
		{
			carSelected = 0;
		}
		else
		{
			carSelected += dir;
		}
		cars[carSelected].gameObject.SetActive(true);
		onSelectNewCar(carSelected);

		return 0;
	}

	public void onSelectNewCar(int id)
	{
		Debug.Log (id);
	}

	/*
	public void drag(float d)
	{
		if(cars[carSelected] != null)
		{
			Quaternion q = cars[carSelected].gameObject.transform.rotation;
			Vector3 e = q.eulerAngles;
			e.y += d / 100f;
			q.eulerAngles = e;
			cars[carSelected].gameObject.transform.rotation = q;
		}
	}
	*/

	public CarID getCurrentCarId()
	{
		return cars[carSelected].carId;
	}

	public void stopSound()
	{
		carSoundStart.Stop();
		carSoundIdle.Stop();
	}

	void Update () 
	{
		if(!carIdle)
		{
			idleSoundDelay -= Time.deltaTime;
			if(idleSoundDelay < 0f)
			{
				carSoundIdle.Play();
				carIdle = true;
			}
		}
	}
}
