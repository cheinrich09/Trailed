using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Require a character controller to be attached to the same game object
[RequireComponent(typeof(CharacterMotor))]
[AddComponentMenu("Character/FPS Input Control")]

public class FPSInputControl : MonoBehaviour
{
    private CharacterMotor motor;
	public GameObject bSpawn;
	//public int BulletIndex;
	public int Score;
	public bool cheatEnabled;
	public bool isHunter;
	int cDelay;
	
	private Ray topRay;
	private Ray bottomRay;
	public GameObject TopHitObject;
	public GameObject BottomHitObject;

	//public int Health;
	//public GameObject[] Bullets;
	//public GameObject BulletPrefab;

    // Use this for initialization
    void Awake()
    {
        motor = GetComponent<CharacterMotor>();
		//BulletIndex = 0;
		Score = 0;
		cDelay = 15;
		//Bullets = new GameObject[10];
		
		if(networkView.isMine)
		{
			Screen.showCursor = false; 
			Camera.main.GetComponentInChildren<SmoothFollow>().target=transform;	
		}
		//bSpawn = this.gameObject.transform.FindChild("BulletSpawn").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
		if(networkView.isMine)
		{
       		// Get the input vector from kayboard or analog stick
        	Vector3 directionVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			Debug.DrawRay(transform.position, transform.forward);
			cDelay--;
       		if (directionVector != Vector3.zero)
        	{
        	    // Get the length of the directon vector and then normalize it
            	// Dividing by the length is cheaper than normalizing when we already have the length anyway
            	float directionLength = directionVector.magnitude;
            	directionVector = directionVector / directionLength;

            	// Make sure the length is no bigger than 1
            	directionLength = Mathf.Min(1.0f, directionLength);
	
        	    // Make the input vector more sensitive towards the extremes and less sensitive in the middle
        	    // This makes it easier to control slow speeds when using analog sticks
        	    directionLength = directionLength * directionLength;

	            // Multiply the normalized direction vector by the modified length
    	        directionVector = directionVector * directionLength;
    	    }

        	// Apply the direction to the CharacterMotor
        	motor.inputMoveDirection = transform.rotation * directionVector;
        	motor.inputJump = Input.GetButton("Jump");
	
			//topRay = new Ray (transform.position, transform.forward);
			
			if(isHunter && Input.GetMouseButtonDown(0))// && cDelay == 0)
			{
				//Debug.Log("Fire Bullet");
				//GameObject.Find ("GameGO").GetComponent<NetworkManager>().FireBullet(this.gameObject);
				//Debug.Log ("Attempt to Capture Enemy");
				//Object Players[] = FindObjectsOfType("Player");
				//cDelay = 15;
				topRay = new Ray (new Vector3(transform.position.x, transform.position.y+0.5f, transform.position.z), transform.forward);
				bottomRay = new Ray (new Vector3(transform.position.x, transform.position.y-0.5f, transform.position.z), transform.forward);
				TopHitObject = null;
				BottomHitObject = null;
				RaycastHit hit;

				if (Physics.Raycast(topRay,out hit, 5))
				{
					
					//hitObject = TopHit.rigidbody.gameObject;
					TopHitObject = hit.collider.gameObject;
					if(TopHitObject.tag == "Player")
					{
						GameObject.Find ("GameGO").GetComponent<NetworkManager>().onHunterCatch(this.gameObject, TopHitObject);
					}
				}
				if (Physics.Raycast(bottomRay, out hit, 5))
				{
					BottomHitObject = hit.collider.gameObject;
					if (BottomHitObject.tag == "Player")
					{
						GameObject.Find ("GameGO").GetComponent<NetworkManager>().onHunterCatch(this.gameObject, BottomHitObject);
					}
				}
				//if(Input.GetMouseButtonDown(0))
				//{		
				//}
				/*else if (Physics.Raycast(bottomOrigin, transform.forward, hit, 10))
				{
					
				}*/
				//if(TopHit.rigidbody.gameObject.tag == "Player")
				//{
				//	
				//}
				
				
			}
		}
    }
	
	void OnCollisionEnter(Collision collision)
	{
		//GameObject.Find ("GameGO").GetComponent<NetworkManager>().onBulletCollide(this.gameObject, collision);
		if(networkView.isMine)
		{
			//GameObject.Find ("GameGO").GetComponent<NetworkManager>().onHunterCollide(this.gameObject, collision);
		}
	}
	
	/*public GameObject FireBullet()
	{
		//return (GameObject)Network.Instantiate(PlayerPrefab,
		//	sPoint.transform.position, Quaternion.identity, 0);	
	}*/
}