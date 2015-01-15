using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GuiShowroom : GuiScreen
{
	public Text carText;

	void Start () 
	{
	}
	
	void Update () 
	{
	}

	public override void init()
	{
		manager.showroomManager.gameObject.SetActive(true);
		manager.showroomManager.switchCar(0);
		carText.text = manager.showroomManager.getCarName();
	}
	
	public override void end()
	{
		manager.showroomManager.gameObject.SetActive(false);
		manager.showroomManager.stopSound();
	}
	
	public void click(int id)
	{
		switch(id)
		{
		case 0: manager.showroomManager.switchCar(-1); carText.text = manager.showroomManager.getCarName(); break;
		case 1: manager.showroomManager.switchCar(1); carText.text = manager.showroomManager.getCarName(); break;
		case 2: manager.switchGui("main"); break;
		}
	}
	
	public override string getGuiName()
	{
		return "showroom";
	}
}
