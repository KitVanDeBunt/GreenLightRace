using UnityEngine.UI;

public class GuiOptions : GuiScreen 
{
    [UnityEngine.SerializeField]
    private InputField playerNameInput;
    [UnityEngine.SerializeField]
    private Text playerNamePlaceholder;

    public override void init() 
	{
        playerNamePlaceholder.text = Settings.Player.name; 
	}

    //called in scene
    public void OnPlayerNameTextChange()
    {
        Settings.Player.name = playerNameInput.text;
    }

    public override GuiScreenId GetGuiId()
    {
        return GuiScreenId.Options;
    }
}
