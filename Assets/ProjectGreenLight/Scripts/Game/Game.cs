using UnityEngine;

class Game
{
    public static void SpawnPlayer(CarList carList, CarType carType, Transform SpawnPoint, VehicleFollow cam)
    {
        GameObject newCar = (GameObject)Network.Instantiate(carList.cars[0].gameObject, SpawnPoint.position, SpawnPoint.rotation, 0);
        if(carType == CarType.self)
        {
            cam.target = newCar.GetComponent<CarInfo>().follow;
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
}
