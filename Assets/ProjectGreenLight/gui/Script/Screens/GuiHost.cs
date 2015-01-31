using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GuiHost : GuiScreen 
{
	public InputField field;

    internal override void OnNetEvent(Events.Net message)
    {
        switch (message)
        {
            case Events.Net.SERVER_INIT:
                ServerInit();
                break;
        }
    }

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

	public void startServer()
	{
        Settings.Player.roomname = field.text;
        if (field.text != "")
        {
            EventManager.callOnGuiEvent(Events.GUI.START_SERVER);
        }
		//Debug.Log("server started - name:" + roomName);
	}

    public override GuiScreenId GetGuiId()
    {
        return GuiScreenId.Host;
    }

    void ServerInit()
    {
        manager.switchGui(GuiScreenId.LobbyServer); 
    }
}
