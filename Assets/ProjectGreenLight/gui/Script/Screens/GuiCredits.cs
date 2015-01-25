using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GuiCredits : GuiScreen 
{
	void Start () 
	{
	}
	
	void Update () 
	{
	}

	public void back()
	{
		manager.switchGui(GuiScreenId.Main);
	}

    public override GuiScreenId GetGuiId()
	{
        return GuiScreenId.Credits;
	}
}
