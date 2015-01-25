using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GuiLobbyClient : GuiLobbyBase
{
    public override GuiScreenId GetGuiId()
    {
        return GuiScreenId.LobbyClient;
    }
}
