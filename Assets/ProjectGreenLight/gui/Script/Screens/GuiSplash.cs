using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GuiSplash : GuiScreen 
{
	public Image background;
	public Image grain;
	public Image fade;

	private float f = -1f;
	private float time = -1.5f;

	public AudioSource soundProjector;
	
	public RectTransform Image_splash;
	public RectTransform Image_grain;
	public RectTransform Image_fade;

	private float delay;
	private float skip;

	void Start () 
	{
		background.color = new Color (1f, 1f, 1f, f);
		grain.color = new Color (1f, 1f, 1f, f);
		fade.color = new Color (1f, 1f, 1f, f);
		soundProjector.Play();

		delay = 0f;
		skip = 0.05f;
	}
	
	void Update () 
	{
		time += Time.deltaTime * 1.7f;
		delay += Time.deltaTime;
		if(delay > skip)
		{
			delay -= skip;
			Image_grain.anchoredPosition = new Vector2 (Random.Range(-300f, 300f), Random.Range(-400f, 400f));
			Image_fade.anchoredPosition = new Vector2 (Random.Range(-10f, 10f), Random.Range(-10f, 10f));

			if(f < 1f && Random.Range(0f, 1f) > 0.6f)
			{
				Image_splash.anchoredPosition = new Vector2 (Random.Range(-240f * (1f - f), 240f * (1f - f)), 0f);
			}
			else
			{
				Image_splash.anchoredPosition = new Vector2 (0f, 0f);
			}
		}

		if(time < 3.25f)
		{
			f = time;
		}
		else
		{
			f = 6.5f - time;
		}

		if(f >= 0f && f <= 1f)
		{
			background.color = new Color (1f, 1f, 1f, f);
			grain.color = new Color (1f, 1f, 1f, f);
			fade.color = new Color (1f, 1f, 1f, f);
		}

		if(time > 8f)
		{
			manager.switchGui(GuiScreenId.Main);
		}
	}

    public override GuiScreenId GetGuiId()
    {
        return GuiScreenId.Splash;
    }
}
