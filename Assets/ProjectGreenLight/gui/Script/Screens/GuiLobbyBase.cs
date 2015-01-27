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
                newItem.transform.Translate(0F, (-15F+ ((float)i * -30F)), 0F);
                NetworkPlayerNoir iPlayer = netPlayerList[i];
                newItem.textPlayerName.text = iPlayer.name;
                //newItem.textPlayerPing.text = Network.connections[i].
                if (iPlayer.state == NetworkPlayerNoirState.ready)
                {
                    newItem.readyToggleButton.targetGraphic.color = newItem.colorReady;
                    Debug.Log("Player: "+iPlayer.name+" -- ready");
                }
                else
                {
                    newItem.readyToggleButton.targetGraphic.color = newItem.colorNotReady;
                    Debug.Log("Player: " + iPlayer.name + " -- not ready");
                }
                //
                if (iPlayer.netPlayer == Network.player)
                {
                    //set self graphic
                    newItem.selfGraphic.gameObject.SetActive(true);
                    //ready toggle
                    newItem.readyToggleButton.enabled = true;
                    newItem.readyToggleButton.onClick.AddListener(
                        delegate
                        {
                            ToggleReady();
                        }
                    );
                    //
                    newItem.kickButton.gameObject.SetActive(false);
                }
                else
                {
                    newItem.selfGraphic.gameObject.SetActive(false);
                    if (Network.isServer)
                    {
                        newItem.readyToggleButton.enabled = false;

                        newItem.kickButton.gameObject.SetActive(true);
                        newItem.kickButton.onClick.AddListener(
                            delegate
                            {
                                KickPlayer(iPlayer);
                            });
                    }
                    else
                    {
                        newItem.readyToggleButton.enabled = false;
                        newItem.kickButton.gameObject.SetActive(false);
                    }
                }
                newItem.textPlayerPing.text = "ping?";
            }
        }
        else
        {
            loadingText.gameObject.SetActive(true);
            //EventManager.callOnGuiEvent(Events.GUI.REFRESH);
        }
    }

    void KickPlayer(NetworkPlayerNoir player)
    {
        Game.KickPlayer(player);
    }

    void ToggleReady()
    {
        Game.ToggleReady();
    }

    public override abstract GuiScreenId GetGuiId();

}
