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
	//public int Health;
	//public GameObject[] Bullets;
	//public GameObject BulletPrefab;

    // Use this for initialization
    void Awake()
    {
        motor = GetComponent<CharacterMotor>();
		//BulletIndex = 0;
		Score = 0;
		//Bullets = new GameObject[10];
		if(networkView.isMine)
		{
			Camera.main.GetComponentInChildren<SmoothFollow>().target=transform;	
		}
		bSpawn = this.gameObject.transform.FindChild("BulletSpawn").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
		if(networkView.isMine)
		{
       		// Get the input vector from kayboard or analog stick
        	Vector3 directionVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			Debug.DrawRay(transform.position, transform.forward);
	
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
			if(Input.GetMouseButtonDown(0))
			{
				Debug.Log("Fire Bullet");
				GameObject.Find ("GameGO").GetComponent<NetworkManager>().FireBullet(this.gameObject);
				
			}
		}
    }
	
	/*public GameObject FireBullet()
	{
		//return (GameObject)Network.Instantiate(PlayerPrefab,
		//	sPoint.transform.position, Quaternion.identity, 0);	
	}*/
}