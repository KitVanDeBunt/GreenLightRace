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
		case 0: startServer(field.text); break;
		case 1: manager.switchGui("multiplayer"); break;
		}
	}

	public void startServer(string roomName)
	{
        Game.netMain.NetEvent(Events.GUI_START_SERVER, roomName);
		Debug.Log(roomName);
	}
	
	public override string getGuiName()
	{
		return "host";
	}

    //Called on the server whenever a Network.InitializeServer was invoked and has completed.
    void OnServerInitialized()
    {
        manager.switchGui("lobbyserver"); 
    }

    //Called on the client when you have successfully connected to a server.
    void OnConnectedToServer()
    {
    }

    
}
