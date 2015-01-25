using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GuiMultiplayer : GuiScreen 
{
    public override void switchGui(GuiScreen guiScreen)
	{
        GuiScreenId nextScreenId = guiScreen.GetGuiId();
        switch (nextScreenId)
		{
            case GuiScreenId.Host:
                EventManager.callOnGuiEvent(Events.GUI.SERVER_MENU);
                manager.switchGui(GuiScreenId.Host); 
                break;

            case GuiScreenId.ServerList:
                EventManager.callOnGuiEvent(Events.GUI.MENU_SERVERLIST);
                manager.switchGui(GuiScreenId.ServerList); 
                break;

            case GuiScreenId.Direct:
                manager.switchGui(GuiScreenId.Direct);
                ((GuiDirect)manager.getMenuById(GuiScreenId.Direct)).returnGui = 0; 
                break;

            case GuiScreenId.Main: 
                manager.switchGui(GuiScreenId.Main); 
                break;

            default:
                base.switchGui(nextScreenId);
                break;

		}
	}

    public override GuiScreenId GetGuiId()
    {
        return GuiScreenId.MultiPlayer;
    }
}
