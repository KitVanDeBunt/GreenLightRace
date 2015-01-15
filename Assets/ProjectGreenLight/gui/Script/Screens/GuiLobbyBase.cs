using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

// baseclass for GuiLobbyClient and GuiLobbyServer
public class GuiLobbyBase : GuiScreen
{
    [SerializeField]
    private Text loadingText;
    [SerializeField]
    private Image serverListPanel;
    [SerializeField]
    private GuiLobbyItem itemPrefab;


    private HostData[] hostList;
    private List<GuiLobbyItem> playerDisplayList;

    public override void init()
    {
        DrawPlayerList();
    }

    public void click(int id)
    {
        switch (id)
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

    private void DrawPlayerList()
    {
        loadingText.gameObject.SetActive(false);
        playerDisplayList = new List<GuiLobbyItem>();
        for (int i = 0; i < Network.connections.Length; i++)
        {
            //create item
            GuiLobbyItem newItem = ((GameObject)GameObject.Instantiate(itemPrefab.gameObject, Vector3.zero, Quaternion.identity)).GetComponent<GuiLobbyItem>();
            playerDisplayList.Add(newItem.GetComponent<GuiLobbyItem>());
            //position item
            newItem.transform.SetParent(serverListPanel.transform, false);
            newItem.transform.Translate(0F, ((1f + (float)i) * -75F), 0F);
            newItem.textPlayerName.text = "player name";
            //newItem.textPlayerPing.text = Network.connections[i].
            newItem.textPlayerReady.text = "ready?";
            newItem.textPlayerPing.text = "ping?";
        }
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
}
