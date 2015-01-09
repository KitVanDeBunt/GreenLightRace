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
        Network.natFacilitatorIP = NetSettings.FACILITATOR_IP;
        Network.natFacilitatorPort = NetSettings.FACILITATOR_PORT;
        MasterServer.ipAddress = NetSettings.MASTER_SERVER_IP;
        MasterServer.port = NetSettings.MASTER_SERVER_PORT;
    }

    public virtual void Close()
    {
        Network.Disconnect();
    }
}
