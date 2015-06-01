#if !UNITY_WEBGL
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
	private int ping_ = 0;
	private NetworkPlayer netPlayer_;
	private CarID carId_;
	private NetworkPlayerNoirState state_;

	public NetworkPlayerNoir (string name, NetworkPlayerNoirState state, NetworkPlayer netPlayer)
	{
		name_ = name;
		netPlayer_ = netPlayer;
		state_ = state;
		Debug.Log ("new player name:" + name_);
	}

	public int ping {
		get {
			return ping_;
		}
		set {
			ping_ = value;
		}
	}

	public string name {
		get {
			//Debug.Log("get player name:" + name_);
			return name_;
		}
		set {
			//Debug.Log("set player name:" + value);
			name_ = value;
		}
	}

	public NetworkPlayerNoirState state {
		get {
			// Debug.Log("get player state" + state_);
			return state_;
		}
		set {
			//Debug.Log("set player state:" + value);
			state_ = value;
		}
	}



	public CarID carId {
		get {
			return carId_;
		}
		set {
			carId_ = value;
		}
	}

	public NetworkPlayer netPlayer {
		get {
			return netPlayer_;
		}
		set {
			netPlayer_ = value;
		}
	}

}
#endif
