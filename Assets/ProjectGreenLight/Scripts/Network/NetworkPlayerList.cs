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
        playerList_ = new List<NetworkPlayerNoir>();
    }

    private bool CheckIfPlayerInPlayerList(NetworkPlayer player)
    {
        for (int i = 0; i < playerList_.Count; i++)
        {
            if (playerList_[i].netPlayer == player)
            {
                return true;
            }
        }
        return false;
    }

    public NetworkPlayerNoir GetNoirNetworkPlayer(NetworkPlayer player)
    {
        for (int i = 0; i < playerList_.Count; i++)
        {
            if (player == playerList_[i].netPlayer)
            {
                return playerList_[i];
            }
        }
        return null;
    }

    public void Loop()
    {
        Ping();
    }

    public void Ping()
    {
        if (Network.isClient)
	    {
            Debug.Log(Network.GetLastPing(Network.connections[0]));
            monoBehaviour_.networkView.RPC("RPCRecievePing", RPCMode.All, Network.player, Network.GetLastPing(Network.connections[0]));
	    }
    }

    public void KickPlayer(NetworkPlayerNoir player)
    {
        Network.CloseConnection(player.netPlayer, true);
    }

    public void ToggleReady()
    {
        monoBehaviour_.networkView.RPC("RPCToggleReady", RPCMode.Others, Network.player);
        RPCToggleReady(Network.player);
    }

    public void RegisterCarID(CarID carId)
    {
        monoBehaviour_.networkView.RPC("RPCRegisterCarID", RPCMode.Others, (int)carId);
        RPCRegisterCarID(Network.player, (int)carId);
    }

    public void RPCRegisterCarID(NetworkPlayer player, int id)
    {
        NetworkPlayerNoir noirPlayer = GetNoirNetworkPlayer(player);
        if (noirPlayer != null)
        {
            noirPlayer.carId = (CarID)id;
            Debug.Log(noirPlayer.carId+"");
        }
        EventManager.callOnNetEvent(Events.Net.NEW_PLAYER_LIST);
    }

    public void RPCRecievePing(NetworkPlayer player, int ping)
    {
        NetworkPlayerNoir noirPlayer = GetNoirNetworkPlayer(player);
        if (noirPlayer != null)
        {
            noirPlayer.ping = ping;
            Debug.Log(noirPlayer.name+" p: "+ noirPlayer.ping);
        }
    }

    public void RPCToggleReady(NetworkPlayer player)
    {
        NetworkPlayerNoir noirPlayer = GetNoirNetworkPlayer(player);
        if (noirPlayer != null)
        {
            if (NetworkPlayerNoirState.notReady == noirPlayer.state)
            {
                noirPlayer.state = NetworkPlayerNoirState.ready;
            }
            else if (NetworkPlayerNoirState.ready == noirPlayer.state)
            {
                noirPlayer.state = NetworkPlayerNoirState.notReady;
            }
            EventManager.callOnNetEvent(Events.Net.NEW_PLAYER_LIST);
        }
    }

    public void RPCSetPlayerState(NetworkPlayer player, int state)
    {
        if (CheckIfPlayerInPlayerList(player))
        {
            //register state
            NetworkPlayerNoir noirPlayer = GetNoirNetworkPlayer(player);
            if (noirPlayer != null)
            {
                noirPlayer.state = (NetworkPlayerNoirState)state;
            }
            if (Network.isServer)
            {
                //register state client at other clients
                monoBehaviour_.networkView.RPC("RPCSetPlayerState", RPCMode.Others, player, (int)state);
            }
            //call event to update ui
            EventManager.callOnNetEvent(Events.Net.NEW_PLAYER_LIST);
        }
    }

    public void RPCRegisterPlayer(NetworkPlayer newPlayer, string newPlayername, int state)
    {
        if (!CheckIfPlayerInPlayerList(newPlayer))
        {
            bool isNotNewPlayer = (newPlayer != Network.player);
            //register players on clients
            if (Network.isServer && isNotNewPlayer)
            {
                for (int i = 0; i < playerList_.Count; i++)
                {
                    //register player list at new client
                    monoBehaviour_.networkView.RPC("RPCRegisterPlayer", newPlayer, playerList_[i].netPlayer, playerList_[i].name, (int)playerList_[i].state);
                    if (playerList_[i].netPlayer != Network.player)
                    {
                        //register new client at playerlist exept server
                        monoBehaviour_.networkView.RPC("RPCRegisterPlayer", playerList_[i].netPlayer, newPlayer, newPlayername, (int)state);
                    }
                }
                //register new client at new client
                monoBehaviour_.networkView.RPC("RPCRegisterPlayer", newPlayer, newPlayer, newPlayername, (int)state);
            }
            //register player
            NetworkPlayerNoir newPlayerNoir = new NetworkPlayerNoir(newPlayername, (NetworkPlayerNoirState)state, newPlayer);
            newPlayerNoir.carId = CarID.thomasCar;
            playerList_.Add(newPlayerNoir);
            //call event to update ui
            EventManager.callOnNetEvent(Events.Net.NEW_PLAYER_LIST);
        }
    }

    public void RPCUnregisterPlayer(NetworkPlayer player)
    {
        //unregiste
        NetworkPlayerNoir noirPlayer = GetNoirNetworkPlayer(player);
        if (noirPlayer != null)
        {
            playerList_.Remove(noirPlayer);
             Debug.Log("removePlayer name:" + noirPlayer.name);
        }
        if (Network.isServer)
        {
            //unregister at clients
            monoBehaviour_.networkView.RPC("RPCUnregisterPlayer", RPCMode.Others, player);
        }
        //
        EventManager.callOnNetEvent(Events.Net.NEW_PLAYER_LIST);
    }
}