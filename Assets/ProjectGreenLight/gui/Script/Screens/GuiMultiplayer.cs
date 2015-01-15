using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GuiMultiplayer : GuiScreen 
{
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
            Game.netMain.NetEvent(Events.GUI_SERVER_MENU);
            manager.switchGui("host"); 
            break;

		case 1:
            Game.netMain.NetEvent(Events.GUI_CLIENT_MENU);
            manager.switchGui("serverlist"); 
            break;

		case 2: 
            manager.switchGui("direct"); 
            ((GuiDirect)manager.getMenuByName("direct")).returnGui = 0; 
            break;

		case 3: 
            manager.switchGui("main"); 
            break;

		}
	}
	
	public override string getGuiName()
	{
		return "multiplayer";
	}
}
