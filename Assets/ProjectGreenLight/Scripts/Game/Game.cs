using UnityEngine;
using System.Collections.Generic;

class Game : MonoBehaviour
{
	private static Game game_;
	private static bool gameFound_;
	[SerializeField]
	private WorldPath worldPath_;

    private static List<CarInfo> playerList_ = new List<CarInfo>();
    private static CarInfo currentCar;
    private static VehicleFollow cam_;
	 
    public static void SpawnPlayer(CarList carList,int carNum, CarType carType, Transform SpawnPoint, VehicleFollow cam)
    {
        cam_ = cam;
        GameObject newCar = (GameObject)Network.Instantiate(carList.cars[carNum].gameObject, SpawnPoint.position, SpawnPoint.rotation, 0);
        playerList_.Add(newCar.GetComponent<CarInfo>());
        if(carType == CarType.self)
        {
            currentCar = newCar.GetComponent<CarInfo>();
            cam.target = currentCar.follow;
            newCar.gameObject.AddComponent<CarPlayer>();
        }
        else if (carType == CarType.other)
        {
            //newCar.gameObject.AddComponent<CarPlayer>();
        }
        else if (carType == CarType.aI)
        {
            newCar.gameObject.AddComponent<CarAI>();
        }
    }
    
	public static Game game
	{
		get
		{
			if(!gameFound_){
				game_ = GameObject.Find("Game").GetComponent<Game>();
				gameFound_ = true;
			}
			return game_;
		}
	}

    private int NextCarID()
    {
        int nextNum = 0;
        for (int i = 0; i < playerList_.Count; i++)
        {
            if (currentCar == playerList_[i])
            {
                nextNum = i;
                break;
            }
        }
        nextNum++;
        if (nextNum > playerList_.Count-1)
        {
            nextNum = 0;
        }
        Debug.Log(nextNum);
        return nextNum;
    }

    private void SwitchCam(int num)
    {
        currentCar = playerList_[num];
        cam_.target = currentCar.follow;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (cam_ != null)
            {
                SwitchCam(NextCarID());
            }
        }
    }
    
    public WorldPath worldPath
    {
    	get
    	{
			return worldPath_;
    	}
    }
}
