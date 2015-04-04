using UnityEngine;
using System.Collections;

public class ShowroomManager : MonoBehaviour
{
    public float rotateSpeed = 20;

    public ShowroomCar[] cars;
    public Quaternion[] spawnRotations;
    public Vector3[] spawnPositions;
    public int carSelected;
    public int carLength;

    public AudioSource carSoundStart;
    public AudioSource carSoundIdle;
    private float idleSoundDelay;
    private bool carIdle = true;

    public void init()
    {
        carSelected = 0;

        cars = GetComponentsInChildren<ShowroomCar>(true);
        spawnRotations = new Quaternion[cars.Length];
        spawnPositions = new Vector3[cars.Length];
        carLength = cars.Length;

        for (int i = 0; i < carLength; i++)
        {
            cars[i].gameObject.SetActive(false);
            spawnRotations[i] = cars[i].transform.rotation;
            spawnPositions[i] = cars[i].transform.position;
        }
    }

    public int switchCar(int dir)
    {
        stopSound();
        idleSoundDelay = 2.5f;
        carIdle = false;
        carSoundStart.Play();

        cars[carSelected].gameObject.SetActive(false);
        if (carSelected + dir < 0)
        {
            carSelected = carLength - 1;
        }
        else if (carSelected + dir >= carLength)
        {
            carSelected = 0;
        }
        else
        {
            carSelected += dir;
        }
        cars[carSelected].gameObject.SetActive(true);
        cars[carSelected].transform.rotation = spawnRotations[carSelected];
        cars[carSelected].transform.position = spawnPositions[carSelected];
        cars[carSelected].GetComponent<Rigidbody>().velocity = Vector3.zero;
        onSelectNewCar(carSelected);

        return 0;
    }

    public void onSelectNewCar(int id)
    {
        Debug.Log(id);
    }

    public void drag(float d)
    {
        if(cars[carSelected] != null)
        {
            Quaternion q = cars[carSelected].gameObject.transform.rotation;
            Vector3 e = q.eulerAngles;
            e.y += d;
            q.eulerAngles = e;
            cars[carSelected].gameObject.transform.rotation = q;
        }
    }

    public CarID getCurrentCarId()
    {
        return cars[carSelected].carId;
    }

    public void stopSound()
    {
        carSoundStart.Stop();
        carSoundIdle.Stop();
    }


    bool mousedown = false;
    Vector3 oldPos;

    void Update()
    {
        if (!carIdle)
        {
            idleSoundDelay -= Time.deltaTime;
            if (idleSoundDelay < 0f)
            {
                carSoundIdle.Play();
                carIdle = true;
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            mousedown = true;
            oldPos = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            mousedown = false;
        }
        if (Input.GetMouseButton(0))
        {
            if (mousedown)
            {
                float deltaPos = Input.mousePosition.x - oldPos.x;
                deltaPos = ((deltaPos / Screen.width) * Time.deltaTime * rotateSpeed);
                drag(deltaPos);
            }
            oldPos = Input.mousePosition;
        }
    }
}
