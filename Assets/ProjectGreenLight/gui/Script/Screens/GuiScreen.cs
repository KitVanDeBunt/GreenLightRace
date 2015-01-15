using UnityEngine;
using System.Collections;

public class GuiScreen : MonoBehaviour 
{
	[HideInInspector]
	public GuiManager manager;

	public void add(GuiManager m)
	{
		manager = m;
	}

	public virtual void init()
	{
	}

	public virtual void end()
	{
	}

	public virtual string getGuiName()
	{
		return "null";
	}

	public void playButtonClick()
	{
		manager.soundButtonClick.Play();
	}

	public void playButtonHover()
	{
		manager.soundButtonHover.Play();
	}
}
