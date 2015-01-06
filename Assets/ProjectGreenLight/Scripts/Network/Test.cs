using UnityEngine;
using System.Collections;

public enum NetworkState{
	newInstance,
	server,
	client1,
	client2
}

public class NetworkStateManager{
	private NetworkState state_ = NetworkState.newInstance;
	private MonoBehaviour caller_;
	
	public NetworkStateManager(MonoBehaviour caller){
		caller_ = caller;
	}
	
	public NetworkState state{
		get{
			return state_;
		}
		set{
			caller_.BroadcastMessage("OnNetworkStateChange");
			state_ = value;
		}
	}
}

public class Test : MonoBehaviour {	
	const int MAX_PLAYERS = 4;
	const int SERVER_PORT = 4000;
	const string MASTER_SERVER_IP = "127.0.0.1";
	const int MASTER_SERVER_PORT = 23466;
	const string GAME_TYPE = "kitvandebunt_testgame";
	
	private int playerCount = 0;
	private NetworkStateManager nsm;
	private string DisplayText;
	
	void Start(){
		nsm	= new NetworkStateManager(this);
	}
	
	void OnGUI(){
		switch(nsm.state){
		case NetworkState.newInstance:
			if(GUI.Button(new Rect(10,10,150,20),"Server")){
				StartServer ();
				nsm.state = NetworkState.server;
			};
			if(GUI.Button(new Rect(10,50,150,20),"Client")){
				Debug.Log("\nClient Button");
				nsm.state = NetworkState.client1;
			};
			break;
		case NetworkState.server:
			if(GUI.Button(new Rect(10,10,150,20),"Host List Test")){
				//MasterServer.RequestHostList();
			};
			break;
		case NetworkState.client1:
			if(GUI.Button(new Rect(10,10,150,20),"List Servers")){
				
			};
			if(GUI.Button(new Rect(10,50,150,20),"Join Top Server")){
				nsm.state = NetworkState.client2;
			};
			break;
		}
	}
	
	void ShowHostList(){
		
	}
	
	void OnNetworkStateChange(){
		Debug.Log("\nstateChange");
	}
	
	void StartServer (){
		bool useNat = !Network.HavePublicAddress();
		Network.InitializeServer(MAX_PLAYERS,SERVER_PORT,useNat);
		MasterServer.ipAddress = MASTER_SERVER_IP;
		MasterServer.port= MASTER_SERVER_PORT;
		MasterServer.RegisterHost(GAME_TYPE,"testing","hello testing");
		Debug.Log("\nMasterServer.ipAddress :" +  MasterServer.ipAddress);
		Debug.Log("\nMasterServer.port :" + MasterServer.port);
	}
	
	//Called on clients or servers when there is a problem connecting to the master server.
	void OnFailedToConnectToMasterServer(NetworkConnectionError info) {
		Debug.Log("\nCould not connect to master server: " + info);
	}
	
	//Called on clients or servers when reporting events from the MasterServer.
	void OnMasterServerEvent(MasterServerEvent msEvent) {
		if (msEvent == MasterServerEvent.HostListReceived){
			Debug.Log("\nMasterServerEvent.HostListReceived");
		}else if(msEvent == MasterServerEvent.RegistrationFailedGameName){
			Debug.Log("\nMasterServerEvent.RegistrationFailedGameName");
		}else if(msEvent == MasterServerEvent.RegistrationFailedGameType){
			Debug.Log("\nMasterServerEvent.RegistrationFailedGameType");
		}else if(msEvent == MasterServerEvent.RegistrationFailedNoServer){
			Debug.Log("\nMerServerEvent.RegistrationFailedNoServer");
		}else if(msEvent == MasterServerEvent.RegistrationSucceeded){
			Debug.Log("\nMasterServerEvent.RegistrationSucceeded");
		}
		
	}
	
	//Called on the server whenever a Network.InitializeServer was invoked and has completed.
	void OnServerInitialized () {
		Debug.Log("\nServer initialized and ready");
	}
	
	//Called on the client when you have successfully connected to a server.
	void OnConnectedToServer (){
		Debug.Log("\nConnected to server");
	}	
	
	//Called on client during disconnection from server, but also on the server when the connection has disconnected.
	void OnDisconnectedFromServer (NetworkDisconnection info){
		if (Network.isServer){
			Debug.Log("\nLocal server connection disconnected");
		}else{
			if (info == NetworkDisconnection.LostConnection){
				Debug.Log("\nLost connection to the server");
			}else{
				Debug.Log("\nSuccessfully diconnected from the server");
			}
		}
	}	
	
	//Called on the client when a connection attempt fails for some reason.
	void OnFailedToConnect (NetworkConnectionError error) {
		Debug.Log("\nCould not connect to server: " + error);
	}
	
	//Called on objects which have been network instantiated with Network.Instantiate.			
	void OnNetworkInstantiate (NetworkMessageInfo info) {
		Debug.Log("\nNew object instantiated by " + info.sender);
	}
	
	//Called on the server whenever a new player has successfully connected.		
	void OnPlayerConnected  (NetworkPlayer player) {
		playerCount++;
		Debug.Log("\nPlayer " + playerCount + " connected from " + player.ipAddress + ":" + player.port);
	}
	
	//Called on the server whenever a player is disconnected from the server.
	void OnPlayerDisconnected  (NetworkPlayer player) {
		Debug.Log("\nClean up after player " + player);
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}
	
	//Used to customize synchronization of variables in a script watched by a network view.
	/*void OnSerializeNetworkView (BitStream stream, NetworkMessageInfo info) {
		int health = 0;
		if (stream.isWriting) {
			health = currentHealth;
			stream.Serialize(ref health);
		} else {
			stream.Serialize(ref health);
			currentHealth = health;
		}
	}*/	
}

