using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GuiOptions : GuiScreen 
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
		case 0: manager.switchGui("main"); break;
		}
	}
	
	public override string getGuiName()
	{
		return "options";
	}
}
