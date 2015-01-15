using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GuiDirect : GuiScreen 
{
	public int returnGui = 0;

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
		case 0: connect(field.text); break;
		case 1: if(returnGui == 0) { manager.switchGui("multiplayer"); } else { manager.switchGui("serverlist"); } break;
		}
	}

	public void connect(string ip)
	{
		Debug.Log(ip);
	}
	
	public override string getGuiName()
	{
		return "direct";
	}
}
