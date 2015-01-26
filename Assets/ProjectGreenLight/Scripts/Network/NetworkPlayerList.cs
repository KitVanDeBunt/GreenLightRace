using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayerList
{
    private List<NetworkPlayerNoir> playerList_;

    private MonoBehaviour monoBehaviour_;

    public NetworkPlayerNoir[] playerList
    {
        get{
            if (playerList_ == null)
            {
                return null;
            }
            else if (playerList_.Count == 0)
            {
                return null;
            }
            else
            {
                return playerList_.ToArray();
            }
        }
    }
    public NetworkPlayerList(MonoBehaviour monoBehaviour)
    {
        monoBehaviour_ = monoBehaviour;
    }

    public void RPCRegisterPlayer(NetworkPlayer newPlayer, string newPlayername, int state)
    {
        if (playerList_ == null)
        {
            playerList_ = new List<NetworkPlayerNoir>();
        }
        bool playerNotInList = true;
        for (int i = 0; i < playerList_.Count; i++)
        {
            if (playerList_[i].netPlayer == newPlayer)
            {
                playerNotInList = false;
                break;
            }
        }
        if (playerNotInList)
        {
            //register at server
            NetworkPlayerNoir newPlayerNoir = new NetworkPlayerNoir(newPlayername, (NetworkPlayerNoirState)state, newPlayer);
            playerList_.Add(newPlayerNoir);
            //register at clients
            if (Network.isServer && newPlayer != Network.player)
            {
                //register server at new client
                monoBehaviour_.networkView.RPC("RPCRegisterPlayer", newPlayer, Network.player, Settings.Player.name, (int)NetworkPlayerNoirState.joined);
                //register new client at other clients
                monoBehaviour_.networkView.RPC("RPCRegisterPlayer", RPCMode.Others, newPlayer, newPlayername, (int)state);
            }
            //
            EventManager.callOnNetEvent(Events.Net.NEW_PLAYER_LIST);
        }
    }

    public void RPCUnregisterPlayer(NetworkPlayer player)
    {
        Debug.Log("removePlayer");
        //unregiste at server
        for (int i = 0; i < playerList_.Count; i++)
        {
            if (player == playerList_[i].netPlayer)
            {
                playerList_.RemoveAt(i);
                Debug.Log("removePlayer n:"+i);
            }
        }
        //unregister at clients
        if (Network.isServer)
        {
            monoBehaviour_.networkView.RPC("RPCUnregisterPlayer", RPCMode.Others, player);
        }
        //
        EventManager.callOnNetEvent(Events.Net.NEW_PLAYER_LIST);
    }
}