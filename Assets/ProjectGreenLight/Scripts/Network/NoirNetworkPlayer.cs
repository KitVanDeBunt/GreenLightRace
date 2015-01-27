using UnityEngine;

public enum NetworkPlayerNoirState
{
    notReady,
    ready,
    inGame
}

public class NetworkPlayerNoir
{
    private string name_;
    private NetworkPlayer netPlayer_;
    private NetworkPlayerNoirState state_;

    public NetworkPlayerNoir(string name, NetworkPlayerNoirState state, NetworkPlayer netPlayer)
    {
        name_ = name;
        netPlayer_ = netPlayer;
        state_ = state;
        Debug.Log("new player name:" + name_);
    }

    public string name
    {
        get
        {
            Debug.Log("get player name:" + name_);
            return name_;
        }
        set
        {
            Debug.Log("set player name:" + value);
            name_ = value;
        }
    }

    public NetworkPlayerNoirState state
    {
        get
        {
            Debug.Log("get player state" + state_);
            return state_;
        }
        set
        {
            Debug.Log("set player state:" + value);
            state_ = value;
        }
    }

    public NetworkPlayer netPlayer
    {
        get
        {
            return netPlayer_;
        }
        set
        {
            netPlayer_ = value;
        }
    }

}
