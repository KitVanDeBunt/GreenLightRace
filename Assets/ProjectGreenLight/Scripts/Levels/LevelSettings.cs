using UnityEngine;
using System.Collections;

public class LevelSettings : MonoBehaviour
{
    [SerializeField]
    private WorldPath worldPath_;
    [SerializeField]
    private Transform[] spawns_;
    [SerializeField]
    private CarList carList;
    [SerializeField]
    private VehicleFollow cam;

    private void Start()
    {
        if (Network.peerType != NetworkPeerType.Disconnected)
        {
            if (Network.isServer)
            {
                Game.SpawnPlayer(carList, 1, CarType.self, spawns_[0], cam);
                Game.SpawnPlayer(carList, 0, CarType.aI, spawns_[5], cam);
            }
            else
            {
                Game.SpawnPlayer(carList, 1, CarType.self, spawns_[1], cam);
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


    public Transform[] spawns
    {
        get
        {
            return spawns_;
        }
    }
}
