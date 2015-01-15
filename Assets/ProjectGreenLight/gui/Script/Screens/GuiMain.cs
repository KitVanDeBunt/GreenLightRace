using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GuiMain : GuiScreen 
{
	private float fade0;
	private float fade1;
	private float fade2;

	public Image background;
	public Image logoImage;
	public Image button_Multiplayer_Image;
	public Image button_Options_Image;
	public Image button_Credits_Image;

	private bool playing;

	void Start () 
	{
		background.color = new Color (1f, 1f, 1f, 0f);
		logoImage.color = new Color (1f, 1f, 1f, 0f);
		button_Multiplayer_Image.color = new Color (1f, 1f, 1f, 0f);
		button_Options_Image.color = new Color (1f, 1f, 1f, 0f);
		button_Credits_Image.color = new Color (1f, 1f, 1f, 0f);
		
		fade1 = -0.2f;
		fade1 = -0.4f;
		fade2 = -1.25f;
		playing = false;
	}

	void Update () 
	{
		if(fade1 < 2f)
		{
			fade0 += Time.deltaTime * 2f;
			fade1 += Time.deltaTime * 0.8f;
			fade2 += Time.deltaTime * 1.4f; 
			
			if(fade0 > 0f && fade0 <= 1f)
			{
				if(fade0 > 0.3f && !playing)
				{
					manager.musicBackground.Play();
				}
				background.color = new Color (1f, 1f, 1f, fade0);
			}

			if(fade1 > 0f && fade1 <= 1f)
			{
				logoImage.color = new Color (1f, 1f, 1f, fade1);
			}
			
			if(fade2 > 0f && fade2 <= 1f)
			{
				button_Multiplayer_Image.color = new Color (1f, 1f, 1f, fade2);
				button_Options_Image.color = new Color (1f, 1f, 1f, fade2);
				button_Credits_Image.color = new Color (1f, 1f, 1f, fade2);
			}

			if(fade2 >= 2f)
			{
				background.color = new Color (1f, 1f, 1f, 1f);
			}
		}
	}

	public void click(int id)
	{
		switch(id)
		{
		case 0: manager.switchGui("multiplayer"); break;
		case 1: manager.switchGui("options"); break;
		case 2: manager.switchGui("credits"); break;
		}
	}

	public override string getGuiName()
	{
		return "main";
	}
}
