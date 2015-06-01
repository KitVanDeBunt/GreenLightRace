#if !UNITY_WEBGL
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GuiLobbyServer : GuiLobbyBase
{
	public override GuiScreenId GetGuiId ()
	{
		return GuiScreenId.LobbyServer;
	}
}
#endif
