using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GuiCredits : GuiScreen 
{
	public void back()
	{
		manager.switchGui(GuiScreenId.Main);
	}

    public override GuiScreenId GetGuiId()
	{
        return GuiScreenId.Credits;
	}
}
