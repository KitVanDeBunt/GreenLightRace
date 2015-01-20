using UnityEngine;

public class NetInstance
{
    internal NetworkStateManager nsm_;

    public NetInstance(NetworkStateManager nsm)
    {
        nsm_ = nsm;
    }

    public virtual void Init()
    {
        Network.natFacilitatorIP = Settings.Net.FACILITATOR_IP;
        Network.natFacilitatorPort = Settings.Net.FACILITATOR_PORT;
        MasterServer.ipAddress = Settings.Net.MASTER_SERVER_IP;
        MasterServer.port = Settings.Net.MASTER_SERVER_PORT;
    }

    public virtual void Close()
    {
        Network.Disconnect();
    }
}
