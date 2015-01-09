using UnityEngine;

class Server : NetInstance
{
    private string gameName_;
    public Server(NetworkStateManager nsm,string gameName) :base(nsm)
    {
        gameName_ = gameName;
    }

    public override void Init()
    {
        base.Init();
        Debug.Log("\nServer Start");

        bool useNat = !Network.HavePublicAddress();
        Network.InitializeServer(NetSettings.MAX_PLAYERS, NetSettings.SERVER_PORT, useNat);

        MasterServer.RegisterHost(NetSettings.GAME_TYPE, gameName_, "hello testing");
        Debug.Log("\nMasterServer.ipAddress :" + MasterServer.ipAddress);
        Debug.Log("\nMasterServer.port :" + MasterServer.port);
    }

    public override void Close()
    {
        base.Close();
    }
}
