using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

// baseclass for GuiLobbyClient and GuiLobbyServer
public abstract class GuiLobbyBase : GuiScreen
{
    [SerializeField]
    private Text loadingText;

    //ui player list
    [SerializeField]
    private Image serverListPanel;
    [SerializeField]
    private Image serverListPanelParent;
    [SerializeField]
    private GuiLobbyItem itemPrefab;
    [SerializeField]
    private Scrollbar scrollBarMessageList;

    //ui chat box
    [SerializeField]
    private Image chatBoxPanel;
    [SerializeField]
    private Image ChatBoxPanelParent;
    [SerializeField]
    private GuiLobbyChatItem chatItemPrefab;
    [SerializeField]
    private InputField chatInput;

    private HostData[] hostList;
    private List<GuiLobbyItem> playerDisplayList;
    private List<GuiLobbyChatItem> chatDisplayList;

    internal override void OnNetEvent(Events.Net message)
    {
        switch (message)
        {
            case Events.Net.NEW_PLAYER_LIST:
                DrawPlayerList();
                break;
            case Events.Net.NEW_MESSAGE:
                AddMessage();
                break;
        }
    }

    public override void init()
    {
        DrawPlayerList();
        chatInput.onEndEdit.AddListener(
                            delegate
                            {
                                OnChatInputEnd();
                            });
        chatDisplayList = new List<GuiLobbyChatItem>();
    }

    public override void end()
    {
        RemovePlayerDisplayList();
        ClearChatBoxDisplayList();
        SetRectTransformVerticalListHeight(serverListPanel.rectTransform, 0, 30F, serverListPanelParent.rectTransform);
        SetRectTransformVerticalListHeight(chatBoxPanel.rectTransform, 0, 20F, ChatBoxPanelParent.rectTransform);
    }

    void OnChatInputEnd()
    {
        //input
        string input = chatInput.text;
        if (chatInput.text != "")
        {
            chatInput.text = "";
            Game.SendChatMessage(input); 
        }
    }

    private void AddMessage()
    {
        GuiLobbyChatItem newItem = ((GameObject)GameObject.Instantiate(chatItemPrefab.gameObject, Vector3.zero, Quaternion.identity)).GetComponent<GuiLobbyChatItem>();
        Noir.Network.Message[] messages = Noir.Network.ChatManager.messages;
        Noir.Network.Message message = messages[messages.Length - 1];
        newItem.text.text = (message.messageOrigin + ": " + message.message);
        chatDisplayList.Add(newItem.GetComponent<GuiLobbyChatItem>());
        //position item
        newItem.transform.SetParent(chatBoxPanel.transform, false);
        newItem.transform.Translate(0F, (-10F + ((float)chatDisplayList.Count * -20F)), 0F);
        //self icon
        newItem.selfIcon.gameObject.SetActive(message.self);
        //chatmessage box size 
        SetRectTransformVerticalListHeight(chatBoxPanel.rectTransform, chatDisplayList.Count, 20F, ChatBoxPanelParent.rectTransform);
        //scroll to bottom
        //scrollBarMessageList.value = 0;
        ChatBoxPanelParent.gameObject.GetComponent<ScrollRect>().velocity = new Vector2(0, 1000f);
        

        //UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(chatInput.gameObject, null);
        //chatInput.OnPointerClick(new PointerEventData(EventSystem.current));
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

    private void ClearChatBoxDisplayList()
    {
        if (chatDisplayList != null)
        {
            int loopCount = chatDisplayList.Count - 1;
            for (int i = loopCount; i > -1; i--)
            {
                GameObject.Destroy(chatDisplayList[i].gameObject);
                chatDisplayList.RemoveAt(i);
            }
        }
    }

    private void SetRectTransformVerticalListHeight(RectTransform rectTrans, int lenght, float itemHeight, RectTransform minHeight)
    {
        float newHeight = (itemHeight * lenght);
        float parentHeight = RectTransformUtil.GetHeight(minHeight);
        if (parentHeight > newHeight)
        {
            newHeight = parentHeight;
        }
        RectTransformUtil.SetHeight(rectTrans, newHeight);
    }

    private void DrawPlayerList()
    {
        NetworkPlayerNoir[] netPlayerList = Game.netPlayerList;
        //list background size
        if (netPlayerList != null)
        {
            SetRectTransformVerticalListHeight(serverListPanel.rectTransform,netPlayerList.Length,30F,serverListPanelParent.rectTransform);
        }

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
                newItem.player = netPlayerList[i];
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
                newItem.textPlayerPing.text = Network.GetLastPing(netPlayerList[i].netPlayer).ToString();
            }
        }
        else
        {
            loadingText.gameObject.SetActive(true);
            //EventManager.callOnGuiEvent(Events.GUI.REFRESH);
        }
    }

    void Update(){
        PlayerListUpdate();
    }

    void PlayerListUpdate()
    {
        if (playerDisplayList != null)
        {
            for (int i = 0; i < playerDisplayList.Count; i++)
            {
                //update ping
                playerDisplayList[i].textPlayerPing.text = playerDisplayList[i].player.ping.ToString();
            } 
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
