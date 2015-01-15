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
		manager.switchGui("main");
	}
	
	public override string getGuiName()
	{
		return "credits";
	}
}
