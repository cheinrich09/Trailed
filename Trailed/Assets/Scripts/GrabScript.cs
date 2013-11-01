using UnityEngine;
using System.Collections;

public class GrabScript : MonoBehaviour {
	
	public GameObject[] players;
	public int throwingForce = 50;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//if(holdingBall)
		//{
			if(Input.GetKeyDown(KeyCode.F))
			{
				
				//heldBall.rigidbody.velocity = transform.TransformDirection(new Vector3(0, 5, throwingForce));
				//holdingBall = false;
				//heldBall.GetComponent<BallScript>().held = false;
				//heldBall.GetComponent<BallScript>().ballActive = true;
				//heldBall = null;
			}
		//}
		//else
		//{
			if(Input.GetKeyDown(KeyCode.E))
			{
				for(int i = 0; i < players.Length; i++)
				{
					if(Vector3.Distance(transform.position, players[i].transform.position) < 10)
					{
						Debug.Log("Caught");
						break;
					}
				}
			}
		//}
	}
}
