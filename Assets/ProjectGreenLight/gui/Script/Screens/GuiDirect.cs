using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GuiDirect : GuiScreen 
{
	public int returnGui = 0;

	public InputField field;
	
	/*public void click(int id)
	{
		switch(id)
		{
		case 0: connect(field.text); break;
		case 1: if(returnGui == 0) { manager.switchGui("multiplayer"); } else { manager.switchGui("serverlist"); } break;
		}
	}*/

    public override void switchGui(GuiScreen guiScreen)
    {
        GuiScreenId nextScreenId =guiScreen.GetGuiId();
        switch (nextScreenId)
        {
            case GuiScreenId.LobbyClient:
                Console.Log("work In Progress");
                switchGui(nextScreenId);
                break;

            default:
                switchGui(nextScreenId);
                break;

        }
    }

	public void connect(string ip)
	{
		Debug.Log(ip);
	}

    public override GuiScreenId GetGuiId()
    {
        return GuiScreenId.Direct;
    }
}
