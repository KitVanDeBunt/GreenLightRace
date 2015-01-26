using UnityEngine;
using System.Collections;

public abstract class GuiScreen : MonoBehaviour 
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

    void OnEnable()
    {
        EventManager.OnNetEvent += OnNetEvent;
        //Debug.Log("[" + GetGuiId().ToString() + "] On enable");
    }

    void OnDisable()
    {
        EventManager.OnNetEvent -= OnNetEvent;
        //Debug.Log("[" + GetGuiId().ToString() + "] On disable");
    }

    internal virtual void OnNetEvent(Events.Net message)
    {
    }

    public abstract GuiScreenId GetGuiId();

    public virtual void switchGui(GuiScreen screen)
    {
        manager.switchGui(screen.GetGuiId());
    }

    public void switchGui(GuiScreenId id)
    {
        manager.switchGui(id);
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
