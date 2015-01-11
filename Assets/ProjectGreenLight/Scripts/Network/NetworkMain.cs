using UnityEngine;
using System.Collections;

public class NetworkMain : MonoBehaviour
{
    //game
    [SerializeField]
    private CarList carList;
    [SerializeField]
    private VehicleFollow cam;
    [SerializeField]
    private Transform spawnServer;
    [SerializeField]
    private Transform spawnClient;
    [SerializeField]
    private Transform spawnAI;

    //network
    private int playerCount = 0;
    private NetworkStateManager nsm;
    private NetInstance netInstance;
    private HostData[] hostList;
    private bool refreshing = false;

    //ui
    private const float buttonY = 10F;
    private const float buttonDist = 40F;
    private int buttonNum = 0;
    private string gameName = "Room Name";
    private string DisplayText;
    private Console console;

    void Start()
    {
        nsm = new NetworkStateManager(this);
        console = gameObject.GetComponent<Console>();
    }

    Rect ButtonRect(float posStart, float posDelta, int num)
    {
        buttonNum += 1;
        float buttonYPos = posStart + (posDelta * (float)num);
        return new Rect(10, buttonYPos, 350, 30);
    }

    void OnGUI()
    {
        buttonNum = 0;
        if (GUI.Button(ButtonRect(buttonY, buttonDist, buttonNum), "Console"))
        {
            console.enabled = !console.enabled;
        };
        switch (nsm.menuState)
        {
            case NetworkMenuState.newInstance:
                // start menu
                if (GUI.Button(ButtonRect(buttonY, buttonDist, buttonNum), "Server"))
                {
                    nsm.menuState = NetworkMenuState.server;
                };
                if (GUI.Button(ButtonRect(buttonY, buttonDist, buttonNum), "Client"))
                {
                    netInstance = new Client(nsm);
                    netInstance.Init();
                    nsm.menuState = NetworkMenuState.client1;
                    nsm.netType = NetworkType.client;
                };
                break;
            case NetworkMenuState.server:
                // server host menu
                gameName = GUI.TextField(ButtonRect(buttonY, buttonDist, buttonNum),gameName);
                if (GUI.Button(ButtonRect(buttonY, buttonDist, buttonNum), "Back"))
                {
                    nsm.menuState = NetworkMenuState.newInstance;
                };
                if (GUI.Button(ButtonRect(buttonY, buttonDist, buttonNum), "Host"))
                {
                    netInstance = new Server(nsm, gameName);
                    netInstance.Init();
                    nsm.netType = NetworkType.server;
                };
                break;

            case NetworkMenuState.server2:
                // menu server
                /*if (GUI.Button(ButtonRect(buttonY, buttonDist, buttonNum), "Refresh"))
                {
                    RefreshHostList();
                }*/
                if (GUI.Button(ButtonRect(buttonY, buttonDist, buttonNum), "Close Server"))
                {
                    netInstance.Close();
                }
                break;

            case NetworkMenuState.client1:
                // menu client
                if (GUI.Button(ButtonRect(buttonY, buttonDist, buttonNum), "Back"))
                {
                    nsm.menuState = NetworkMenuState.newInstance;
                };
                if (GUI.Button(ButtonRect(buttonY, buttonDist, buttonNum), "Refresh"))
                {
                    RefreshHostList();
                };
                DrawHostList();
                break;
            case NetworkMenuState.client2:
                if (GUI.Button(ButtonRect(buttonY, buttonDist, buttonNum), "Close"))
                {
                    nsm.menuState = NetworkMenuState.client1;
                    netInstance.Close();
                };
                break;
        }
    }

    private void RefreshHostList()
    {
        if (!refreshing)
        {
            Console.Log("Refresh Host List");
            MasterServer.RequestHostList(NetSettings.GAME_TYPE);
            refreshing = true;
        }
    }

    private void DrawHostList()
    {
        if (hostList != null)
        {
            for (int i = 0; i < hostList.Length; i++ )
            {
                if (GUI.Button(ButtonRect(buttonY, buttonDist, buttonNum), ("JOIN: "+hostList[i].gameName)))
                {
                    Network.Connect(hostList[i]);
                };
            }
            if (hostList.Length < 1)
            {
                GUI.Label(new Rect(400, 10, 150, 20), "no host found");
            }
            else
            {
                GUI.Label(new Rect(400, 10, 150, 20), hostList.Length + "host(s) found");
            }
        }
        else
        {
            RefreshHostList();
        }
    }
    /*
    private void pingList()
    {
        if (hostList != null)
        {
            for (int i = 0; i < hostList.Length; i++)
            {
                Ping newPing = new Ping(hostList[i].ip);
                };
            }
        }
        else
        {
            RefreshHostList();
        }
    }
    */
    void OnMenuStateChange()
    {
        Console.Log("Menu state change");
    }

    //Called on clients or servers when there is a problem connecting to the master server.
    void OnFailedToConnectToMasterServer(NetworkConnectionError info)
    {
        Console.Log("Could not connect to master server: " + info);
    }

    //Called on clients or servers when reporting events from the MasterServer.
    void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        if (msEvent == MasterServerEvent.HostListReceived)
        {
            Console.Log("MasterServerEvent.HostListReceived");
            hostList = MasterServer.PollHostList();
            Console.Log("Host Count " + hostList.Length);
            refreshing = false;
        }
        else if (msEvent == MasterServerEvent.RegistrationFailedGameName)
        {
            Console.Log("MasterServerEvent.RegistrationFailedGameName");
        }
        else if (msEvent == MasterServerEvent.RegistrationFailedGameType)
        {
            Console.Log("MasterServerEvent.RegistrationFailedGameType");
        }
        else if (msEvent == MasterServerEvent.RegistrationFailedNoServer)
        {
            Console.Log("MerServerEvent.RegistrationFailedNoServer");
        }
        else if (msEvent == MasterServerEvent.RegistrationSucceeded)
        {
            Console.Log("MasterServerEvent.RegistrationSucceeded");
        }

    }

    //Called on the server whenever a Network.InitializeServer was invoked and has completed.
    void OnServerInitialized()
    {
        Console.Log("Server initialized and ready");
        nsm.menuState = NetworkMenuState.server2;
        Game.SpawnPlayer(carList, CarType.self, spawnServer,cam);
        Game.SpawnPlayer(carList, CarType.aI, spawnAI, cam);
    }

    //Called on the client when you have successfully connected to a server.
    void OnConnectedToServer()
    {
        Console.Log("Connected to server");
        nsm.menuState = NetworkMenuState.client2;
        Game.SpawnPlayer(carList, CarType.self, spawnServer, cam);
    }

    //Called on client during disconnection from server, but also on the server when the connection has disconnected.
    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        if (Network.isServer)
        {
            Console.Log("Local server connection disconnected");
        }
        else
        {
            if (info == NetworkDisconnection.LostConnection)
            {
                Console.Log("Lost connection to the server");
            }
            else
            {
                Console.Log("Successfully diconnected from the server");
            }
        }
    }

    //Called on the client when a connection attempt fails for some reason.
    void OnFailedToConnect(NetworkConnectionError error)
    {
        Console.Log("Could not connect to server: " + error);
    }

    //Called on objects which have been network instantiated with Network.Instantiate.			
    void OnNetworkInstantiate(NetworkMessageInfo info)
    {
        Console.Log("New object instantiated by " + info.sender);
    }

    //Called on the server whenever a new player has successfully connected.		
    void OnPlayerConnected(NetworkPlayer player)
    {
        playerCount++;
        Console.Log("Player " + playerCount + " connected from " + player.ipAddress + ":" + player.port);
    }

    //Called on the server whenever a player is disconnected from the server.
    void OnPlayerDisconnected(NetworkPlayer player)
    {
        Console.Log("Clean up after player " + player);
        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);
    }

    //Used to customize synchronization of variables in a script watched by a network view.
    /*void OnSerializeNetworkView (BitStream stream, NetworkMessageInfo info) {
        int health = 0;
        if (stream.isWriting) {
            health = currentHealth;
            stream.Serialize(ref health);
        } else {
            stream.Serialize(ref health);
            currentHealth = health;
        }
    }*/
}

