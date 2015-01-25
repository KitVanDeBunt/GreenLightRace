using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class GuiManager : MonoBehaviour 
{
	private GuiScreen[] guiList;
	private int guiListLength;

	private GuiScreen activeGui;

    [SerializeField]
    private GuiScreenId startScreenId;

	public AudioSource soundButtonClick;
	public AudioSource soundButtonHover;
	public AudioSource musicBackground;

	public EventSystem eventSystem;

	[HideInInspector]
	public ShowroomManager showroomManager;

	void Start () 
	{
		showroomManager = GameObject.Find("ShowroomObject").GetComponent<ShowroomManager>();
		showroomManager.init();
		showroomManager.gameObject.SetActive(false);

		eventSystem = GameObject.Find("Gui_EventSystem").GetComponent<UnityEngine.EventSystems.EventSystem>();

		guiList = gameObject.GetComponentsInChildren<GuiScreen>(true);
		guiListLength = guiList.Length;
		for(int i = 0; i < guiListLength; i++)
		{
            guiList[i].gameObject.SetActive(false);
			guiList[i].add(this);
		}

        switchGui(startScreenId);
	}

	void Update () 
	{
	
	}

	public void switchGui(GuiScreenId id)
	{
		if(activeGui != null)
		{
			activeGui.gameObject.SetActive(false);
			activeGui.end();
		}

		activeGui = getMenuById (id);
		if(activeGui != null)
		{
			activeGui.gameObject.SetActive(true);
			activeGui.init();
		}
	}

    public GuiScreen getMenuById(GuiScreenId n)
	{
		for (int i = 0; i < guiListLength; i++) 
		{
            if (n == guiList[i].GetGuiId()) 
			{
				return guiList[i];
			}
		}
		return null;
	}
}
