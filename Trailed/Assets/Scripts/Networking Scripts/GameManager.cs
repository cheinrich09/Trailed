// Demonstrates a strict authoritative server, where all content is created
// by the server, and interactions between objects are only reacted to on the server (with results sent 
// to the clients via RPC calls).
//
// Some networking content drawn from the "Ultimate Unity networking project" by M2H (http://www.M2H.nl) and 
// from http://www.paladinstudios.com/2013/07/10/how-to-create-an-online-multiplayer-game-with-unity/
// Mostly from Blair MacIntyre, 2013.
// 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	// the information about each host (the server, and each client), stored at each running instance.  
	
	// We could make this different in the client and the server, by subclassing (for example) and 
	// creating a different one in the clients and the server.  
    public class PlayerInfo
    {
		// network name of the player
        public NetworkPlayer networkPlayer;
        public string name;
		
		// we might want to store a bunch of content per client
        public double clickTime;
		public Color color;
		
		// the GameObject that represents this players avatar in this example.  Could obviously store more info here.
		public GameObject go;
		
        public bool IsLocal()
        {
            //If disconnected we are "-1"
            return (Network.player == networkPlayer || Network.player + "" == "-1");
        }
    }
	
	// assume we only have one, keep a link here, so other scripts can call methods
    public static GameManager instance;
	
	// two game content prefabs.  One controlled by the clients, one controlled by a server script.  Would likely have
	// many more.
    public GameObject clientControlledPrefab;
    public GameObject dynamicPrefab;
	public GameObject clientHitCount;
	public GameObject serverHitCount;
	
	// Dictionaries for player information objects, and the list of networked game objects
	public Dictionary<NetworkPlayer,PlayerInfo> playerList = new Dictionary<NetworkPlayer,PlayerInfo>();
	public Dictionary<NetworkViewID,GameObject> myGOList = new Dictionary<NetworkViewID,GameObject>();

	// save a pointer to me, and ensure the network is going!
	void Awake()
    {
        instance = this;
        Network.isMessageQueueRunning = true;
    }

    ////////////////////////////// 
    // Manage players

    [RPC]
    void AddPlayer(NetworkPlayer networkPlayer, string pname, Vector3 color)
    {
        Debug.Log("AddPlayer " + networkPlayer + " name=" + pname);
        if (playerList.ContainsKey(networkPlayer))
        {
            Debug.LogError("AddPlayer: Player " + networkPlayer + " already exists!");
            return;
        }
		
        PlayerInfo pla = new PlayerInfo();
        pla.networkPlayer = networkPlayer;
        pla.name = pname;
		pla.clickTime = 0;
		pla.go = null;
		pla.color = new Color(color.x, color.y, color.z);
		playerList[networkPlayer] = pla;		
    }


    [RPC]
    void RemovePlayer(NetworkPlayer networkPlayer)
    {
        Debug.Log("RemovePlayer " + networkPlayer);
        PlayerInfo thePlayer;

		if (playerList.TryGetValue(networkPlayer, out thePlayer))
        {
			if (thePlayer.go != null)
			{		
				// destroy the player go.
				// if we created it (which is if we are the server), destroy it everywhere
				// if we aren't the server (which is if we are disconnecting ourself), destroy the go because we
				// probably won't get the command back from the server when it executes RemovePlayer
				if (thePlayer.go.networkView.isMine)
				{
					DestroyGameContent(thePlayer.go); 
				} else {
					myGOList.Remove (thePlayer.go.networkView.viewID);			
					Destroy (thePlayer.go);
				}				
			}
			if (Network.isServer)
			{
	        	Network.RemoveRPCs(networkPlayer);
			}
    	    playerList.Remove(networkPlayer);
        } else {
			Debug.Log ("RemovePlayer: player does not exist");
		}
    }
	
	////////////////////////
	// Methods called by other scripts in the game.  Here's a trivial example that stores the last time an
	// interaction event was received by from each player
	public static void SetPlayerClickTime(NetworkPlayer networkPlayer, double time)
	{
		PlayerInfo pInfo;
		if (instance.playerList.TryGetValue(networkPlayer, out pInfo))
		{
			pInfo.clickTime = time;
		} else {
			Debug.Log ("SetPlayerClickTime could not find networkPlayer " + networkPlayer.ToString());
		}
	}
	
    ////////////////////////////
    // Startup and shutdown of client and server, called from network script
	
	// clean up the server.  Remove the network objects it created, which will be recreated by
	// a new server.   Any other server cleanup could be done here
	public void ShutDownServer()
	{
		if (Network.isServer)
		{
			Debug.Log("ShutDownServer");
			
			// create a separate list from the value contents of the dict, so we can delete elements from the dict
			// as we step through if we wanted (ended up not doing that here, just clear all at end).
			GameObject[] arr = new GameObject[myGOList.Count];    
			myGOList.Values.CopyTo(arr, 0);

			foreach (GameObject go in arr)
       	 	{
				if (go.networkView.isMine)
					DestroyGameContent(go);
				else
					Destroy(go);
			}
			myGOList.Clear ();			
		} else {
			Debug.Log ("Called ShutDownServer on CLIENT.  Bad Programmer!");
		}
	}
	
	// shutdown client.  Clean up any data objects created by the server, any other client 
	// cleanup code could be here.
	public void ShutDownClient()
	{
		if (Network.isClient)
		{
			Debug.Log("ShutDownClient");
	        
			// In theory, could call "RemovePlayer" as shown below, but don't need to because the server will call it when
			// we disconnect
			//networkView.RPC("RemovePlayer", RPCMode.All, Network.player);
			
			// create a separate list from the value contents of the dict, so we can delete elements from the dict
			// as we step through.  In particular, DestroyGameContent will remove things from the Dictionary.
			
			// do a network destroy of all the objects I created, and a normal destroy of the others.
			// We do the local Destroy of the others, because the server isn't going to destroy these.
			// The server WILL destroy our avatar object BUT we will probably have disconnected from the server
			// before we get the message.  
			GameObject[] arr = new GameObject[myGOList.Count];    
			myGOList.Values.CopyTo(arr, 0);

			foreach (GameObject go in arr)
       	 	{
				if (go.networkView.isMine)
				{
					DestroyGameContent(go);
				} else {
					// ideally we d
					Destroy(go);
				}
			}
			myGOList.Clear ();
		} else {
			Debug.Log ("Called ShutDownClient on SERVER.  Bad Programmer!");
		}
	}
	
	///////////////////////////
	// Unity networking events on the Server
	
	// when the server is created locally
    void OnServerInitialized()
    {
		// everyone has a copy of the server in their player list.  Buffered so new players get it.
        networkView.RPC("AddPlayer", RPCMode.AllBuffered, Network.player, 
			PlayerPrefs.GetString("playerName"),
			new Vector3(Random.Range(0.3f, 0.5f), Random.Range(0.5f, 1f), Random.Range(0.5f, 1f)));

		// create some server content, with two different behaviors, once the server is up and running
        SpawnGameContent(1, Network.player);
        SpawnGameContent(1, Network.player);
        SpawnGameContent(1, Network.player);
    }

	// when a new client connects.  We don't actually do anything here, we wait for them to "AddPlayer"
    void OnPlayerConnected(NetworkPlayer player)
    {
	}
	
	// When client disconnects, cleanup.  This is called on the server when a client loses connection.  We don't
	// need to buffer this, because the "AddPlayer" will go away when we remove RPCs for that player.
    void OnPlayerDisconnected(NetworkPlayer player)
    {
        networkView.RPC("RemovePlayer", RPCMode.All, player);
    }

	// create the content for this client.  The RPC call is made from the server, because all shared game objects are created
	// in the server and managed there (so the server has control over them!)
	[RPC]
	void CreatePlayerSpecificContent(NetworkPlayer networkPlayer)
	{
		if (!Network.isServer) 
		{
			Debug.Log ("Called CreatePlayerSpecificContent on Client, instead of Server");
			return;	
		}

		if (networkPlayer == Network.player)
		{
			Debug.Log ("Passed Server's network id to CreatePlayerSpecificContent instead of Clients");
			return;				
		}
		
		// For this demo, we just spawn my player
        SpawnGameContent(2, networkPlayer);
	}

	//////////////////////////////
    // Unity networking events on the Client
	
	// when the client successfully connects to the server
    void OnConnectedToServer()
    {
		// add myself to the list of players.   
		// Buffered so future players get it, and the server will cause this call to 
		// no longer be buffered when the player disconnects
        networkView.RPC("AddPlayer", RPCMode.AllBuffered, Network.player, 			
			PlayerPrefs.GetString("playerName"),
			new Vector3(Random.Range(0.35f, 1f), Random.Range(0.2f, 0.5f), Random.Range(0.2f, 0.7f)));
		
		// tell the server to create our content.  We do this separately from "AddPlayer" because 
		// it might eventually vary per client, and we want the eventual calls to create the content items to be 
		// initiated by the server, not the client (AddPlayer will get called on all new clients while we
		// are connected, but those buffered RPCs will disappear when we do)
        networkView.RPC("CreatePlayerSpecificContent", RPCMode.Server, Network.player);
		
		// here, we could do more client-specific things. 
    }
	
	/////////////////////////////////////////////////
	// Creation and destruction of game content. 
	// Two methods for each:
	// 1) SpawnGameContent and DestroyGameContent are the ones that are called locally.  
	//    They will do things that should be the same everywhere (such as allocate data  
	//    or generate randomized values) , and pass them to the RPC function
	// 2) SpawnOnNetwork and DestroyOnNetwork are called locally AND via RPC, with a flag to indicate which.
	//    The RPC is buffered so they are executed on all instances, and on new players as they are added. 
	//    We could pass slightly different parameters locally if we wanted (just check the network flags)  
    void SpawnGameContent(int prefabID, NetworkPlayer player)
    {
        //Spawn local player
        Debug.Log("SpawnGameContent ");

        //Get random spawnpoint
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("Spawnpoint");
        GameObject theGO = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector3 pos = theGO.transform.position;
        Quaternion rot = theGO.transform.rotation;

        // Manually allocate a NetworkViewID.  This setup assumes ALL networking for each conceptual 
		// entity is done via the GO created below in SpawnOnNetwork, since each ViewID should only be used in
		// one GO
        NetworkViewID id1 = Network.AllocateViewID();
		
		// setup things locally, then issue the commands that will be sent to the clients when they
		// connect (this is all being done when the server starts, before the clients connect!)
		// In this simple example, exactly the same thing is done.  BUT, note that we pass in a different
		// value for our parameter "amOwner", so that we can tell if this is being executed here on the server
		// or being executed on the clients after starting
        SpawnOnNetwork(pos, rot, id1, true, player, prefabID);
        networkView.RPC("SpawnOnNetwork", RPCMode.OthersBuffered, pos, rot, id1, false, player, prefabID);
    }

    [RPC]
    void SpawnOnNetwork(Vector3 pos, Quaternion rot, NetworkViewID id1, bool amOwner, NetworkPlayer np, int prefabID)
    {
		GameObject newObject;
        PlayerInfo pNode;
		if (!playerList.TryGetValue(np, out pNode))
		{
			// probably need to do something more drastic here:  this should NEVER happen
			Debug.Log ("SpawnOnNetwork of object #" + prefabID + " with NetWorkID " + id1.ToString() + " for NetworkPlayer " + np.ToString() + " failed, because network Player doesn't exist");
			return;
		}
		
		// would eventually be significantly more complex. Can do different things in the players and server,
		// and could also do different things in each client.  But, this allows each conceptual "entity" 
		// to be created/destroyed by the server in a simple way.
		switch (prefabID) {
		case 1:
			// create a server controlled, wandering cube
			newObject = Instantiate(dynamicPrefab, pos, rot) as GameObject;
	        newObject.renderer.material.color = pNode.color;
			newObject.name = "cube" + id1.ToString();
			
			// add an offset to the script time, so that it's not the same for each.  
			// NOTE:  this only gets used on the Server for Perlin noise (look at the script)
			if (np == Network.player)
			{
				// server controlled object, so only need to set the time offset here
				DynamicObjectScript ds = newObject.GetComponent<DynamicObjectScript>();
				if (ds) 
				{
					ds.offset = Random.Range(0f, 100f);
				} else {
					Debug.Log ("Dynamic Object does not have DynamicObjectScript attached");
				}
			}
			break;
			
		case 2:
			// create the content for the player avatar (another Cube!)
			newObject = Instantiate(clientControlledPrefab, pos, rot) as GameObject;
	        newObject.renderer.material.color = pNode.color;
			newObject.name = "Player" + np.ToString();
			
			// save the player avatar in the player list here on the server
			pNode.go = newObject;

			// make a note of if this is my player (even though it's created by the server)
			PlayerControlledObjectScript ps = newObject.GetComponent<PlayerControlledObjectScript>();
			if (ps) 
			{
				ps.player = np;
				ps.isMyPlayer = (np == Network.player);
				if (ps.isMyPlayer)
				{
					GameObject hc = Instantiate(clientHitCount) as GameObject;
					ps.hitCountText = hc;
				} else if (Network.isServer) {
					// on the server, add a hit count to each player
					ps.hitCountText = Instantiate (serverHitCount) as GameObject;
					ps.hitCountText.transform.parent = newObject.transform;
					ps.hitCountText.transform.localPosition = new Vector3(0f, 1.5f, 0f);
					
					ps.hitCountText.renderer.material.color = pNode.color;
				} else {
					ps.hitCountText = null;
				}
			} else {
				Debug.Log ("Player Object does not have PlayerControlledObjectScript attached");
			}
			break;
			
		default:
			Debug.Log("Invalid prefab ID = " + prefabID);
			return;
		}
		
        // Set networkviewID everywhere in the game object. Just set the one, but it could be the case that eventually we
		// pass more than one along, and then need to set it in here.
        SetNetworkViewIDs(newObject.gameObject, id1);
		
		// keep track of our network game objects in a dictionary, so we can rapidly find objects based on their network
		// view ID.
		// In the "authoritative server" setup here, most (or all) network objects (especially those that can be moved around
		// by the game) are created by the server, but you could imagine the client creating other objects
		// that only they control

		myGOList.Add (id1, newObject);
    }
	
	// Destroy an item of game content.  If there were any RPCs buffered for this object, remove them too.
	void DestroyGameContent(GameObject go)
	{
		// only the owner of the GameObject should destroy it
		if (go.networkView.isMine)
		{
			// will be in the NetGOList, but construction!
			NetworkViewID viewID = go.networkView.viewID;

			Network.RemoveRPCs(viewID);   // get rid of buffered RPC calls for this object, if any

			DestroyOnNetwork(viewID, true);
			networkView.RPC("DestroyOnNetwork", RPCMode.OthersBuffered, viewID, false);
		} else {
			Debug.Log ("DestroyGameContent: not owner of object " + go.ToString());
		}
	}		
	
	[RPC]
	void DestroyOnNetwork(NetworkViewID id, bool amOwner)
	{
		GameObject go;
		// make sure it still exists, just in case we destroyed it locally (e.g., because we shutdown the client
		// and this message got here before the shutdown finished!)
		if (myGOList.TryGetValue(id, out go))
		{
			myGOList.Remove(id);
			Destroy(go);
		} else {
			Debug.Log("Tried to destroy Non-existant object with viewID " + id.ToString());
		}
	}
		
    // When a NetworkView instantiates it has viewID=0 and is unusable.
    // We need to assign the right viewID -on all players(!)- for it to work.
	// We use a function (as opposed to go.networkview.viewID = id1) so that we could scale up to multiple
	// networkViews eventually.
    void SetNetworkViewIDs(GameObject go, NetworkViewID id1)
    {
        Component[] nViews = go.GetComponentsInChildren<NetworkView>();
		
		foreach (NetworkView nv in nViews)
		{		
			nv.viewID = id1;
			break;  // just set the first
		}
    }


	/////////////////////
	// Server OR client: Disconnect
    IEnumerator OnDisconnectedFromServer(NetworkDisconnection info)
    {
        Debug.Log("... Disconnected From Server ...");
        yield return 0;//Wait for actual disconnect
		
        //Remove all players.  We can just clear the list, they should all be garbage collected.  		
		playerList.Clear ();

		// for each GameObject, we need to destroy it, and then clear the list
		foreach( KeyValuePair<NetworkViewID,GameObject> nvGO in myGOList) 
		{
			Destroy (nvGO.Value);
		}
		myGOList.Clear ();
		
		//Other stuff?     
        if (Network.isServer)
        {
            //We shut down our own server          

        }
        else
        {
            if (info == NetworkDisconnection.LostConnection)
            {
                //Debug.LogWarning("Client Lost connection to the server");
            }
            else
            {
                //Debug.LogWarning("Client Successfully disconnected from the server");
            }
        }
    }



}