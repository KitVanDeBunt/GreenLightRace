using UnityEngine;

public enum NetworkMenuState
{
    newInstance,
    server,
    server2,
    client1,
    client2,
    client3
}

public enum NetworkState
{
    connected,
    notConnected
}

public enum NetworkType
{
    server,
    client,
    noNet
}
public class NetworkStateManager
{
    private NetworkMenuState menuState_ = NetworkMenuState.newInstance;
    private NetworkState netState_ = NetworkState.notConnected;
    private NetworkType netType_ = NetworkType.noNet;
    private MonoBehaviour caller_;

    public NetworkStateManager(MonoBehaviour caller)
    {
        caller_ = caller;
    }

    public NetworkMenuState menuState
    {
        get
        {
            return menuState_;
        }
        set
        {
            caller_.BroadcastMessage("OnMenuStateChange");
            menuState_ = value;
        }
    }

    public NetworkState netState
    {
        get
        {
            return netState_;
        }
        set
        {
            netState_ = value;
        }
    }

    public NetworkType netType
    {
        set
        {
            netType_ = value;
        }
        get
        {
            return netType_;
        }
    }
}
