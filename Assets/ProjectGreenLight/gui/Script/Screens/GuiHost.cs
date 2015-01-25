using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GuiHost : GuiScreen 
{
	public InputField field;

	void Start () 
	{
	}
	
	void Update () 
	{
	}
	
	public void click(int id)
	{
		switch(id)
		{
		case 0: 
            startServer(field.text); 
            break;

        case 1: 
            manager.switchGui(GuiScreenId.MultiPlayer); 
            break;
		}
	}

	public void startServer(string roomName)
	{
        Settings.Player.roomname = roomName;
        EventManager.callOnGuiEvent(Events.GUI.START_SERVER);
		//Debug.Log("server started - name:" + roomName);
	}

    public override GuiScreenId GetGuiId()
    {
        return GuiScreenId.Host;
    }

    //Called on the server whenever a Network.InitializeServer was invoked and has completed.
    void OnServerInitialized()
    {
        manager.switchGui(GuiScreenId.LobbyServer); 
    }
}
