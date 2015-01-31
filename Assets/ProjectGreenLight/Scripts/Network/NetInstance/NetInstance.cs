using UnityEngine;

public abstract class NetInstance
{
    internal NetworkStateManager nsm_;
    public NetworkPlayerList playerList;
    public Noir.Network.ChatManager chat;

    public NetInstance(NetworkStateManager nsm, MonoBehaviour monoBehaviour)
    {
        nsm_ = nsm;
        playerList = new NetworkPlayerList(monoBehaviour);
        chat = new Noir.Network.ChatManager(monoBehaviour, playerList);
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

    public virtual void Loop(){
        playerList.Loop();
    }
}
