#if !UNITY_WEBGL
using UnityEngine;

class Server : NetInstance
{
	private string gameName_;
	public Server (NetworkStateManager nsm, MonoBehaviour monoBehaviour, string gameName)
        : base(nsm, monoBehaviour)
	{
		gameName_ = gameName;
	}

	public override void Init ()
	{
		base.Init ();
		Debug.Log ("\nServer Start");

		bool useNat = !Network.HavePublicAddress ();
		Network.InitializeServer (Settings.Net.MAX_PLAYERS, Settings.Net.SERVER_PORT, useNat);

		MasterServer.RegisterHost (Settings.Net.GAME_TYPE, gameName_);
		Debug.Log ("\nMasterServer.ipAddress :" + MasterServer.ipAddress);
		Debug.Log ("\nMasterServer.port :" + MasterServer.port);
	}

	public override void Close ()
	{
		MasterServer.UnregisterHost ();
		base.Close ();
	}
}
#endif
