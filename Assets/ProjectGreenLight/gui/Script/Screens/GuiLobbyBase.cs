using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

// baseclass for GuiLobbyClient and GuiLobbyServer
public abstract class GuiLobbyBase : GuiScreen
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

    public override void end()
    {
        RemovePlayerDisplayList();
    }

    /*public void Refresh()
    {
        EventManager.callOnGuiEvent(Events.GUI.REFRESH);
        DrawPlayerList();
    }*/

    public override void switchGui(GuiScreen newScreen)
    {
        GuiScreenId newScreenId = newScreen.GetGuiId();
        switch (newScreenId)
        {
            case GuiScreenId.MultiPlayer: //back

                RemovePlayerDisplayList();
                EventManager.callOnGuiEvent(Events.GUI.BACK);
                manager.switchGui(GuiScreenId.MultiPlayer);

                break;
        }
    }

    private void RemovePlayerDisplayList()
    {
        if (playerDisplayList != null)
        {
            int loopCount = playerDisplayList.Count - 1;
            for (int i = loopCount; i > -1; i--)
            {
                GameObject.Destroy(playerDisplayList[i].gameObject);
                playerDisplayList.RemoveAt(i);
            }
        }
    }

    private void DrawPlayerList()
    {
        NetworkPlayerNoir[] netPlayerList = Game.netPlayerList;
        if (netPlayerList != null)
        {
            loadingText.gameObject.SetActive(false);
            RemovePlayerDisplayList();

            //new list
            playerDisplayList = new List<GuiLobbyItem>();
            for (int i = 0; i < netPlayerList.Length; i++)
            {
                //create item
                GuiLobbyItem newItem = ((GameObject)GameObject.Instantiate(itemPrefab.gameObject, Vector3.zero, Quaternion.identity)).GetComponent<GuiLobbyItem>();
                playerDisplayList.Add(newItem.GetComponent<GuiLobbyItem>());
                //position item
                newItem.transform.SetParent(serverListPanel.transform, false);
                newItem.transform.Translate(0F, (-31F+((float)i * -35F)), 0F);
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
            //EventManager.callOnGuiEvent(Events.GUI.REFRESH);
        }
    }

    public override abstract GuiScreenId GetGuiId();

}
