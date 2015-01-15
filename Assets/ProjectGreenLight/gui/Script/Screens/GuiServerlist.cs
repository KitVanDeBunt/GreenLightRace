using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GuiServerlist : GuiScreen 
{
    [SerializeField]
    private Text loadingText;
    [SerializeField]
    private Image serverListPanel;
    [SerializeField]
    private GuiServerListItem buttonPrefab;


    private HostData[] hostList;
    private List<GuiServerListItem> hostDisplayList;
    private int hostClickedNum;

    public override void init()
	{
        Game.netMain.RefreshHostList();
	}

	
	public void click(int id)
	{
		switch(id)
		{
		case 0: //refresh
            Game.netMain.RefreshHostList();
            break;

		case 1: //direct
            manager.switchGui("direct"); 
            ((GuiDirect)manager.getMenuByName("direct")).returnGui = 1;
            break;

		case 2: //back
            Game.netMain.NetEvent(Events.GUI_BACK);
            manager.switchGui("multiplayer"); 
            break;

		}
	}

    private void DrawHostList()
    {
        loadingText.gameObject.SetActive(false);
        hostDisplayList = new List<GuiServerListItem>();
        int num = 0;
        for (int i = 0; i < hostList.Length; i++)
        {
            //create item
            GuiServerListItem newItem = ((GameObject)GameObject.Instantiate(buttonPrefab.gameObject, Vector3.zero, Quaternion.identity)).GetComponent<GuiServerListItem>();
            hostDisplayList.Add(newItem.GetComponent<GuiServerListItem>());
            //position item
            newItem.transform.SetParent(serverListPanel.transform, false);
            newItem.transform.Translate(0F, ((1f + (float)i) * -55F), 0F);
            newItem.textPlayerCount.text = hostList[i].connectedPlayers + "/" + hostList[i].playerLimit;
            newItem.TextServerName.text = hostList[i].gameName;
            int loopNum = i;
            newItem.botton.onClick.AddListener(delegate { ServerButtonClicked(loopNum); });
            num++;
        }
    }

    private void ServerButtonClicked(int hostNum)
    {
        Console.Log("clicked:" + hostNum);
        Network.Connect(hostList[hostNum]); 
    }

    public void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        if (msEvent == MasterServerEvent.HostListReceived)
        {
            Console.Log("MasterServerEvent.HostListReceived");
            hostList = MasterServer.PollHostList();
            Console.Log("Host Count " + hostList.Length);
            DrawHostList();
        }
        else if (msEvent == MasterServerEvent.RegistrationFailedGameName)
        {
        }
        else if (msEvent == MasterServerEvent.RegistrationFailedGameType)
        { 
        }
        else if (msEvent == MasterServerEvent.RegistrationFailedNoServer)
        {
        }
        else if (msEvent == MasterServerEvent.RegistrationSucceeded)
        {
        }
    }

    //Called on the client when you have successfully connected to a server.
    void OnConnectedToServer()
    {
        manager.switchGui("lobbyclient"); 
    }

    //Called on client during disconnection from server, but also on the server when the connection has disconnected.
    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        if (Network.isServer)
        {
        }
        else
        {
            if (info == NetworkDisconnection.LostConnection)
            {
            }
            else
            {
            }
        }
    }

    //Called on the client when a connection attempt fails for some reason.
    void OnFailedToConnect(NetworkConnectionError error)
    {
    }

    //Called on objects which have been network instantiated with Network.Instantiate.			
    void OnNetworkInstantiate(NetworkMessageInfo info)
    {
    }

    //Called on the server whenever a player is disconnected from the server.
    void OnPlayerDisconnected(NetworkPlayer player)
    {
    }

	public string[] getServerList()
	{
		return null;
	}
	
	public override string getGuiName()
	{
		return "serverlist";
	}
}
