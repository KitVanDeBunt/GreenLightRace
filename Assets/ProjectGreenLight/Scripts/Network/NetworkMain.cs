using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkMain : LoaderObject
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
    private string gameName = "temp name";
    public List<NetworkPlayerNoir> playerList;


    //ui
    private const float buttonY = 90F;
    private const float buttonDist = 40F;
    private int buttonNum = 0;
    //private string gameName = "Room Name";
    private string DisplayText;
    private Console console;



    void OnEnable()
    {
        EventManager.OnGuiEvent += GuiEvent;
    }

    void OnDisable()
    {
        EventManager.OnGuiEvent -= GuiEvent;
    }

    void Start()
    {
        nsm = new NetworkStateManager(this);
        console = gameObject.GetComponent<Console>();
    }

    void OnGUI()
    {
        buttonNum = 0;
        OnGuiDrawConsole();
        //OnGuiDrawNetMenu();
    }

    void OnGuiDrawConsole()
    {
        if (GUI.Button(ButtonRect(buttonY, buttonDist, buttonNum), "Console"))
        {
            console.enabled = !console.enabled;
        };
    }

    public void GuiEvent(Events.GUI message)
    {
        bool messageSucses = false;
        switch (message)
        {
            case Events.GUI.SERVER_MENU:
                if (nsm.menuState == NetworkState.newInstance)
                {
                    nsm.menuState = NetworkState.server;
                    messageSucses = true;
                }
                break;
            case Events.GUI.START_SERVER:
                if (nsm.menuState == NetworkState.server)
                {
                    netInstance = new Server(nsm, Settings.Player.roomname);
                    netInstance.Init();
                    messageSucses = true;
                }
                break;

            case Events.GUI.MENU_SERVERLIST:
                if (nsm.menuState == NetworkState.newInstance)
                {
                    netInstance = new Client(nsm);
                    netInstance.Init();
                    nsm.menuState = NetworkState.client1;
                    messageSucses = true;
                }
                break;

            case Events.GUI.BACK:
                if (nsm.menuState == NetworkState.server)
                {
                    nsm.menuState = NetworkState.newInstance;
                    messageSucses = true;
                }
                if (nsm.menuState == NetworkState.server2)
                {
                    netInstance.Close();
                    messageSucses = true;
                }
                if (nsm.menuState == NetworkState.client1)
                {
                    nsm.menuState = NetworkState.newInstance;
                    messageSucses = true;
                }
                break;

            case Events.GUI.REFRESH:
                if (nsm.menuState == NetworkState.client1)
                {
                    RefreshHostList();
                    messageSucses = true;
                }
                break;

            case Events.GUI.CLOSE:
                if (nsm.menuState == NetworkState.client2)
                {
                    nsm.menuState = NetworkState.client1;
                    netInstance.Close();
                    messageSucses = true;
                }
                break;
        }
        if (!messageSucses)
        {
            Debug.LogError("NetEvent state error - current state: " + nsm.menuState + " - event: " + message.ToString());
        }
        else
        {
            Debug.Log("NetEvent - current state: " + nsm.menuState + " - event: " + message.ToString());
        }
    }

    public void RefreshHostList()
    {
        if (!refreshing)
        {
            Console.Log("Refresh Host List");
            MasterServer.RequestHostList(Settings.Net.GAME_TYPE);
            refreshing = true;
        }
    }

    private void DrawHostList()
    {
        if (hostList != null)
        {
            for (int i = 0; i < hostList.Length; i++)
            {
                if (GUI.Button(ButtonRect(buttonY, buttonDist, buttonNum), ("JOIN: " + hostList[i].gameName)))
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

    Rect ButtonRect(float posStart, float posDelta, int num)
    {
        buttonNum += 1;
        float buttonYPos = posStart + (posDelta * (float)num);
        return new Rect(10, buttonYPos, 350, 30);
    }

    void OnGuiDrawNetMenu()
    {
        switch (nsm.menuState)
        {
            case NetworkState.newInstance:
                // start menu
                if (GUI.Button(ButtonRect(buttonY, buttonDist, buttonNum), "Server"))
                {
                    nsm.menuState = NetworkState.server;
                };
                if (GUI.Button(ButtonRect(buttonY, buttonDist, buttonNum), "Client"))
                {
                    netInstance = new Client(nsm);
                    netInstance.Init();
                    nsm.menuState = NetworkState.client1;
                };
                break;

            case NetworkState.server:
                // server host menu
                gameName = GUI.TextField(ButtonRect(buttonY, buttonDist, buttonNum),gameName);
                if (GUI.Button(ButtonRect(buttonY, buttonDist, buttonNum), "Back"))
                {
                    nsm.menuState = NetworkState.newInstance;
                };
                if (GUI.Button(ButtonRect(buttonY, buttonDist, buttonNum), "Host"))
                {
                    netInstance = new Server(nsm, gameName);
                    netInstance.Init();
                };
                break;

            case NetworkState.server2:
                // menu server
                //if (GUI.Button(ButtonRect(buttonY, buttonDist, buttonNum), "Refresh"))
               // {
               //     RefreshHostList();
                //}
                if (GUI.Button(ButtonRect(buttonY, buttonDist, buttonNum), "Close Server"))
                {
                    netInstance.Close();
                }
                break;

            case NetworkState.client1:
                // menu client
                if (GUI.Button(ButtonRect(buttonY, buttonDist, buttonNum), "Back"))
                {
                    nsm.menuState = NetworkState.newInstance;
                };
                if (GUI.Button(ButtonRect(buttonY, buttonDist, buttonNum), "Refresh"))
                {
                    RefreshHostList();
                };
                DrawHostList();
                break;

            case NetworkState.client2:
                if (GUI.Button(ButtonRect(buttonY, buttonDist, buttonNum), "Close"))
                {
                    nsm.menuState = NetworkState.client1;
                    netInstance.Close();
                };
                break;
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
            Console.Log("MasterServerEvent.RegistrationFailedNoServer");
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

        nsm.menuState = NetworkState.server2;
        //Game.SpawnPlayer(carList, 1, CarType.self, spawnServer,cam);
        //Game.SpawnPlayer(carList, 0, CarType.aI, spawnAI, cam);

        RPCAddPlayerToList(Network.player, Settings.Player.name, NetworkPlayerNoirState.joined);
    }

    //Called on the client when you have successfully connected to a server.
    void OnConnectedToServer()
    {
        Console.Log("Connected to server");
        nsm.menuState = NetworkState.client2;
       // Game.SpawnPlayer(carList, 1, CarType.self, spawnServer, cam);
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
        for(int i = 0;i < Network.connections.Length;i++)
        {
            networkView.RPC("RPCAddPlayerToList", RPCMode.All, Network.connections[i], Settings.Player.name, NetworkPlayerNoirState.joined);
        }
        networkView.RPC("RPCAddPlayerToList", player, Network.player, Settings.Player.name, NetworkPlayerNoirState.joined);
    }

    //Called on the server whenever a player is disconnected from the server.
    void OnPlayerDisconnected(NetworkPlayer player)
    {
        Console.Log("Clean up after player " + player);
        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);

        networkView.RPC("RPCRemovePlayerFromList", RPCMode.All, player);
    }

    [RPC]
    void RPCAddPlayerToList(NetworkPlayer player, string name, NetworkPlayerNoirState state)
    {
        if (playerList == null)
        {
            playerList = new List<NetworkPlayerNoir>();
        }
        bool playerNotInList = true;
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].netPlayer == player)
            {
                playerNotInList = false;
                break;
            }
        }
        if (playerNotInList)
        {
            NetworkPlayerNoir newPlayer = new NetworkPlayerNoir(name, state, player);
            playerList.Add(newPlayer);
            EventManager.callOnNetEvent(Events.Net.NEW_PLAYER_LIST);
        }
    }

    [RPC]
    void RPCRemovePlayerFromList(NetworkPlayer player)
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            playerList.Remove(playerList[i]);
        }
        EventManager.callOnNetEvent(Events.Net.NEW_PLAYER_LIST);
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

