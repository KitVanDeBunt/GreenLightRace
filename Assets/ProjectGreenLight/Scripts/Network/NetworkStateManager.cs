public enum NetworkState
{
    newInstance,
    host_menu,
    host_lobby,
    client1,
    client2,
    client3
}

public class NetworkStateManager
{
    private NetworkState menuState_ = NetworkState.newInstance;
    private UnityEngine.MonoBehaviour caller_;

    public NetworkStateManager(UnityEngine.MonoBehaviour caller)
    {
        caller_ = caller;
    }

    public NetworkState menuState
    {
        get
        {
            return menuState_;
        }
        set
        {
            //caller_.BroadcastMessage("OnNetStateChange");
            menuState_ = value;
            Console.Log("Menu state change - current state: " + menuState_.ToString());
        }
    }
}
