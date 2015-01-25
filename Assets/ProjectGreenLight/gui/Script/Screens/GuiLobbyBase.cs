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

    internal override void OnNetEvent(Events.Net message)
    {
        switch (message)
        {
            case Events.Net.NEW_PLAYER_LIST:
                DrawPlayerList();
                break;
        }
    }

    public override void init()
    {
        DrawPlayerList();
    }

    public void click(int id)
    {
        switch (id)
        {
            case 0: //refresh
                //Game.netMain.RefreshHostList();
                EventManager.callOnGuiEvent(Events.GUI.REFRESH);
                DrawPlayerList();
                break;

            case 1: //direct
                manager.switchGui(GuiScreenId.Direct);
                ((GuiDirect)manager.getMenuById(GuiScreenId.Direct)).returnGui = 1;
                break;

            case 2: //back
                //Game.netMain.NetEvent(Events.GUI_BACK);
                EventManager.callOnGuiEvent(Events.GUI.BACK);
                manager.switchGui(GuiScreenId.MultiPlayer);
                break;

        }
    }

    private void DrawPlayerList()
    {
        List<NetworkPlayerNoir> netPlayerList = Game.netPlayerList;
        if (netPlayerList == null)
        {
            loadingText.gameObject.SetActive(false);
            playerDisplayList = new List<GuiLobbyItem>();
            for (int i = 0; i < netPlayerList.Count; i++)
            {
                //create item
                GuiLobbyItem newItem = ((GameObject)GameObject.Instantiate(itemPrefab.gameObject, Vector3.zero, Quaternion.identity)).GetComponent<GuiLobbyItem>();
                playerDisplayList.Add(newItem.GetComponent<GuiLobbyItem>());
                //position item
                newItem.transform.SetParent(serverListPanel.transform, false);
                newItem.transform.Translate(0F, ((1f + (float)i) * -65F), 0F);
                NetworkPlayerNoir iPlayer = netPlayerList[i];
                newItem.textPlayerName.text = iPlayer.name;
                //newItem.textPlayerPing.text = Network.connections[i].
                newItem.textPlayerReady.text = "ready?";
                newItem.textPlayerPing.text = "ping?";
            }
        }
        else
        {
            loadingText.gameObject.SetActive(true);
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
