// script for a player-controlled object (their avatar, usually).
// Here we send the player movement commands to the server.  The server will then send back the proper
// position, based on the centralized game simulation.  If the network update isn't fast enough, this would result in
// sluggish movement.  Ideally, you'd want to keep some history, run locally and then jump if there are conflicts 
// (i.e., think how SecondLife does it)

using UnityEngine;
using System.Collections;

public class PlayerControlledObjectScript : MonoBehaviour {
	// our movement vector.
	private Vector3 moveDirection = Vector3.zero;
	
	public float speed = 10f;
	private bool lastNonZero = false;

	// for recording hits from server
	public GameObject hitCountText;
	public int hitCount = 0;

	// who this represents, and is it me? 
	public bool isMyPlayer = false;
	public NetworkPlayer player; 
	
	
	// simple initialization
	void Start () {
		hitCount = 0;

	}
	
	  
	// Clien moves the objects, which makes things responsive, 
	// but sends any commands that modify the controlled object to the server
	void Update () {
		// only the player for whom this is there object gets to move it.
        if (isMyPlayer)
		{
			float xm = Input.GetAxis("Horizontal");
			float ym = Input.GetAxis("Vertical");
			
			// we will set the moveDirection vector, apply changes immediately, 
			// then take the updates from the server as confirmation
			moveDirection = new Vector3(xm,0,ym);

			// when we stop moving, make sure we send a (0,0,0) movement vector
			if (xm != 0f || ym != 0f)
			{
				// actually execute the official movement on the server, not the client!
				networkView.RPC ("movePlayer", RPCMode.Server, moveDirection);		
				lastNonZero = true;
			} else if (lastNonZero) {
				lastNonZero = false;
				networkView.RPC ("movePlayer", RPCMode.Server, moveDirection);		
			}
		}
			
	}
	
	// the server will override client's movements when it sends its results down.  
	void FixedUpdate () {	
		if (Network.isServer) 
		{
			if (moveDirection.magnitude> 0.001) 
			{
	    	    rigidbody.AddForce(100f * moveDirection * speed * Time.deltaTime);
			}
		}
	}
	
	// stream state changes (in this case, position, but it could be anything you want) 
	// executed both on client and server 
	void OnSerializeNetworkView ( BitStream stream ,   NetworkMessageInfo info  ){
		if (stream.isWriting){
			//Executed on the owner of this networkview; 
			//The server sends it's position over the network
			Vector3 pos = transform.position;		
			stream.Serialize(ref pos);//"Encode" it, and send it
					
		} else {
			//Executed on the others; 
			//receive a position and set the object to it
			Vector3 posReceive = Vector3.zero;
			stream.Serialize(ref posReceive); //"Decode" it and receive it
			transform.position = posReceive;
		}
	}
	
	// use the keyboard inputs from the clients to modify the game
	[RPC]
	void movePlayer(Vector3 dir, NetworkMessageInfo info)
	{
		// save some data in the global player info struct
		GameManager.SetPlayerClickTime(info.sender, info.timestamp);
			
		moveDirection = dir;
	}	
	
	// when the server determines the hitcount has changed, send it to everyone.
	// In our simple example, we attach a 3D Text element to each player on the server's screen, 
	// and put a GUIText on the HUD in the client for whom this is their player.
	// Other players do not see this information for other players.
	//
	// So, if there is a GUIText set on this script's hitCountText variable, update it 
	[RPC]
	void updateHitCount (int newHitCount)
	{
		hitCount = newHitCount;
		if (hitCountText)
		{
			GUIText gt = hitCountText.GetComponent<GUIText>();
			if (gt) {
				gt.text = "Hit Count: " + hitCount;
			}
		}
	}

	// need to make sure we destroy the hitCountText (and any other player-specific state) 
	// when we destroy this player
	void OnDestroy()
	{
		if (hitCountText)
			Destroy (hitCountText);
	}

	// in our simple example, we just keep track of the number of collisions.  
	// When we hit another player, we deduct a hit,  when we hit one of the 3 cubes, we increment a hit. 
	// We add or substract one point per contact point.
	//
	// this code only gets run on the server.  We also update the server's TextMesh display here.
    void OnCollisionEnter(Collision collision) 
	{
		if (Network.isServer)
		{
			int newHits = 0;
	        foreach (ContactPoint contact in collision.contacts) {
				if (contact.otherCollider.gameObject.name.Substring(0,6) == "Player") {
					rigidbody.AddForce (contact.normal * 100f);
					newHits --;
				} else if (contact.otherCollider.gameObject.name.Substring(0,4) == "cube") {
					rigidbody.AddForce (contact.normal * 500f);
					newHits ++;
				} else {
					Debug.Log ("Collided with " + contact.otherCollider.gameObject.name);
				}
			}
			
			if (newHits > 0)
			{
				hitCount += newHits;
				TextMesh tm = hitCountText.GetComponent<TextMesh>();
				tm.text = "Player " + player.ToString() + "\n" + hitCount.ToString();
			
				networkView.RPC  ("updateHitCount", RPCMode.AllBuffered, hitCount);
			}
		} 
	}
}