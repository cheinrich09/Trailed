using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
	//----------------- variables ------------------
	
	//network info
	private string connectionIP = "127.0.0.1";
	private int connectionPort = 25005;
	
	//player info
	private string playerName = "No Name";

	string tempName = "";
	private bool makeMeAClient = false;
	
	public string PlayerName { 
		get { return playerName;}
		set { playerName = value; }
	}
	
	
	private string playerColor = "Red";
	public string PlayerColor { 
		get { return playerColor;}
		set { playerColor = value; }
	}
	//connect window setup
	private Rect connectWindowRect;
	private int connectWindowWidth = 320;
	private int connectWindowHeight = 150;
	private int buttonHeight = 45;
	private int leftIndent;
	private int topIndent;
	private string titleMessage = "Connection Setup";

	//connected window setup
	private Rect infoWindowRect;
	private int infoWindowWidth = 180;
	private int infoWindowHeight = 60;
	
	public float BulletForce;
	private bool Victory = false;
	private string VictoryMessage = " is the Winner!";
	
	
	

	//----------------end variables -----------------

	void Start ()
	{
		playerName = PlayerPrefs.GetString("playerName");
		if(playerName == "" || playerName == null) {
			playerName = "No Name";	
		}
		
	}
	
	void startServer ()
	{
		Network.InitializeServer(32, connectionPort, Network.HavePublicAddress());
		Debug.Log("Starting the server");
	}
	
	//----------- handle incoming messages FROM THE SERVER ------------
	
	//-----these messages are sent to the server
	
	//we are informed that we were successful in initializing the server
	void OnServerInitialized ()
	{
		Debug.Log("Server is initialzed.");
		//GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnScript>().spawnPlayer();
		addPlayer();
	}
	
	// we are informed that a player has just connected to us (the server)
	void OnPlayerConnected (NetworkPlayer player)
	{
		//Debug.Log ("Player " + player + " connected from " + player.ipAddress + ":" + player.port);
	}
	
	///-----these messages are sent to the CLIENT
	
	void OnConnectedToServer ()
	{
		Debug.Log ("I'm a client, connected to server");
		addPlayer();
	}
	
	//called on both client AND server
	void OnDisconnectedFromServer (NetworkDisconnection info)
	{
		//reload the application so we can start over
		Application.LoadLevel (Application.loadedLevel);	
	}
	
	public void FireBullet(GameObject Shooter)
	{
		if(!Victory)
		{
			GameObject GO = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnScript>().spawnBullet(Shooter.GetComponent<FPSInputControl>().bSpawn, Shooter);
			networkView.RPC("SetBullet", RPCMode.AllBuffered, GO.networkView.viewID, Shooter.networkView.viewID);
		}
	}
	
	void addPlayer()
	{
		GameObject GO = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnScript>().spawnPlayer();
		Debug.Log ("Call RPC");
		networkView.RPC("SetPlayer", RPCMode.AllBuffered, GO.networkView.viewID, playerName, playerColor);	
		
	}
	
	public void onBulletCollide(GameObject bullet, Collision collision)
	{
		/*if(collision.gameObject.networkView!=null)
		{
			//networkView.RPC ("BulletCollide",RPCMode.AllBuffered, bullet.networkView.viewID, collision.gameObject.networkView.viewID);
		}*/
		
	 	NetworkViewID ShooterID = bullet.GetComponent<BulletScript>().parent.networkView.viewID;
		if(collision.gameObject.tag == "Player" && collision.gameObject!=bullet.GetComponent<BulletScript>().parent)
		{
			networkView.RPC("Scored", RPCMode.AllBuffered, ShooterID, bullet.GetComponent<BulletScript>().parent.GetComponent<FPSInputControl>().Score+1);
			//networkView.RPC("Scored", RPCMode.AllBuffered, ShooterID, bullet.GetComponent<BulletScript>().parent.GetComponent<FPSInputControl>().Score+50);
		}
		Destroy(bullet);
			//networkView.RPC ("DeleteBullet", RPCMode.All, bullet.networkView.viewID);
		//Destroy (bullet);
	}
	
	[RPC]
	void Scored(NetworkViewID ShooterID, int updatedScore)
	{
		NetworkView shooterView = NetworkView.Find(ShooterID);	
		shooterView.gameObject.GetComponent<FPSInputControl>().Score=updatedScore;
		//shooterView.gameObject.GetComponent<FPSInputControl>().Score+=15;
		
		if (shooterView.gameObject.GetComponent<FPSInputControl>().Score >150)
		{
			Victory = true;
			///string messageToSend = (shooterView.gameObject.GetComponent<PlayerLabel>().PlayerName+" is the Winner!");
			networkView.RPC("SendMessageToEveryone", RPCMode.All,VictoryMessage, shooterView.gameObject.GetComponent<PlayerLabel>().PlayerName+" (Winner)");
		}
	}
	
	[RPC]
	void SetBullet(NetworkViewID BulletID, NetworkViewID ShooterID)
	{
		NetworkView bulletView = NetworkView.Find (BulletID);
		NetworkView shooterView = NetworkView.Find(ShooterID);
		bulletView.gameObject.GetComponent<BulletScript>().parent = shooterView.gameObject;
		bulletView.gameObject.rigidbody.velocity = Vector3.zero;
		bulletView.gameObject.rigidbody.AddForce(BulletForce * bulletView.gameObject.transform.forward);
	}

	
	[RPC]
	void SetPlayer(UnityEngine.NetworkViewID gViewID, string gName, string gColor)
	{
		//gView.gameObject.name = gName;
		NetworkView gView = NetworkView.Find(gViewID);
		//gView.gameObject.name = gName;
		//gView.gameObject.GetComponent<PlayerLabel>().PlayerName = "Player "+gView.viewID;
		//gView.gameObject.name = gName;
		gView.gameObject.GetComponent<PlayerLabel>().PlayerName = gName;
		gView.gameObject.GetComponent<PlayerLabel>().PlayerColor = gColor;
		if (gColor == "Red")
		{
			gView.gameObject.GetComponentInChildren<MeshRenderer>().material = Resources.Load("Materials/Red") as Material;
		}
		else if (gColor == "Orange")
		{
			gView.gameObject.GetComponentInChildren<MeshRenderer>().material = Resources.Load("Materials/Orange") as Material;
		}
		else if (gColor == "Blue")
		{
			gView.gameObject.GetComponentInChildren<MeshRenderer>().material = Resources.Load("Materials/Blue") as Material;
		}
		else if (gColor == "Green")
		{
			gView.gameObject.GetComponentInChildren<MeshRenderer>().material = Resources.Load("Materials/Green") as Material;
		}
		else if (gColor == "Black")
		{
			gView.gameObject.GetComponentInChildren<MeshRenderer>().material = Resources.Load("Materials/Black") as Material;
		}
		Debug.Log("RPC Ran");
	}
	
	//some player has disconnected. 
	//We'd better clean up their stuff
	void OnPlayerDisconnected (NetworkPlayer player)
	{
		Debug.Log ("Clean up after player " + player);
		Network.RemoveRPCs (player);
		Network.DestroyPlayerObjects (player);
	}
	
	//----------- end incoming messages from SERVER ------------
	
	void SetNameWindow (int windowID)
	{
		GUILayout.Label( "Enter your player name: ");
		
		tempName = GUILayout.TextField(tempName, 25);
		
		if(GUILayout.Button("Set Name", GUILayout.Height (buttonHeight))){
			if(playerName != ""){
				playerName = tempName;
				PlayerPrefs.SetString("playerName", playerName);
			}
		}
	}
	
	void SetColorWindow (int windowID)
	{
		GUILayout.Label( "Select your player color: ");
		if (GUILayout.Button("Red", GUILayout.Height (buttonHeight)))
		{
			playerColor = "Red";
		}
		if (GUILayout.Button("Blue", GUILayout.Height (buttonHeight)))
		{
			playerColor = "Blue";
		}
		if (GUILayout.Button("Green", GUILayout.Height (buttonHeight)))
		{
			playerColor = "Green";
		}
		if (GUILayout.Button("Orange", GUILayout.Height (buttonHeight)))
		{
			playerColor = "Orange";
		}
		if (GUILayout.Button("Black", GUILayout.Height (buttonHeight)))
		{
			playerColor = "Black";
		}
	}
	
	void ServerConnectWindow (int windowID)
	{
		GUILayout.Space(15);
		
		GUILayout.Label( "Player Name: "+playerName);
		
		if (GUILayout.Button ("Change Player Name", GUILayout.Height (buttonHeight))) {
			playerName = "No Name";	
		}
		
		if (GUILayout.Button ("Change Player Color", GUILayout.Height (buttonHeight))) {
			playerColor = "No Color";
		}
		
		if (GUILayout.Button("Start Server", GUILayout.Height(buttonHeight)))
		{
			startServer();
		}
		if (GUILayout.Button("Join the Game", GUILayout.Height(buttonHeight)))
		{
			Debug.Log("I wanna join as client");
			makeMeAClient = true;
		}
		
	
	}
	
	void ClientLoginWindow (int windowID)
	{
		GUILayout.Label("Enter Server IP");
		connectionIP = GUILayout.TextField(connectionIP);
		
		GUILayout.Label("Enter Server Port Number");
		connectionPort = int.Parse(GUILayout.TextField(connectionPort.ToString()));
		
		GUILayout.Space(20);
		
		if (GUILayout.Button("Login", GUILayout.Height(buttonHeight), GUILayout.Height(buttonHeight)))
		{
			if(playerName != "")
			{
				Network.Connect(connectionIP, connectionPort);
			}
			else
			{
				playerName = "No Name";
			}
		}
		
		if (GUILayout.Button("Go Back"))
		{
			makeMeAClient = false;
		}
		
		
	}
	
	void ServerInfoWindow (int windowID)
	{
		GUILayout.Space(10);
		if (GUILayout.Button("Shut Down Server"))
		{
			Network.Disconnect();	
		}
	}

	void ClientInfoWindow (int windowID)
	{
		GUILayout.Space(10);
		if (GUILayout.Button("Disconnect"))
		{
			Network.Disconnect();	
		}
	}
	
	void OnGUI ()
	{
		leftIndent = Screen.width / 2 - connectWindowWidth / 2;	
		topIndent = Screen.height / 3 - connectWindowHeight / 2;
		connectWindowRect = new Rect (leftIndent, topIndent, connectWindowWidth, 
					connectWindowHeight);
		if(playerName == "No Name")
		{
			//SetNameWindow(0);
			connectWindowRect = GUILayout.Window (0, connectWindowRect, SetNameWindow, titleMessage);
		}
		else if(playerColor == "No Color")
		{
			//SetColorWindow(0);
			connectWindowRect = GUILayout.Window (0, connectWindowRect, SetColorWindow, titleMessage);
		}
		else
		{
		//first - I'll need to be server
		if (Network.peerType == NetworkPeerType.Disconnected && !makeMeAClient) {
			//create the window
			connectWindowRect = GUILayout.Window (1, connectWindowRect, ServerConnectWindow, titleMessage);
			 
		}
		
		// I do wanna be client, show login dialog
		if (Network.peerType == NetworkPeerType.Disconnected && makeMeAClient) {
			//create the window
			connectWindowRect = GUILayout.Window (2, connectWindowRect, ClientLoginWindow, "Login");	
		}
		
		if (Network.peerType == NetworkPeerType.Server) {
			infoWindowRect = new Rect (20, 20, infoWindowWidth, infoWindowHeight);
			infoWindowRect = GUILayout.Window (3, infoWindowRect, ServerInfoWindow, ("Connected as: " + playerName));
			
		}
		
		if (Network.peerType == NetworkPeerType.Client) {
			
			infoWindowRect = new Rect (20, 20, infoWindowWidth, infoWindowHeight);
			infoWindowRect = GUILayout.Window (4, infoWindowRect, ClientInfoWindow, ("Connected as: " + playerName));

		}
		}
	}
}
