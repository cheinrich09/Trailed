using UnityEngine;
using System.Collections;

public class GrabScript : MonoBehaviour {
	
	public GameObject hand;
	public bool holdingBall;
	private CharacterMotor motor;
	public RaycastHit hit;
	public GameObject[] balls;
	
	public int throwingForce = 50;
	
	// Use this for initialization
	void Start () {
		holdingBall = false;
		
		motor = GetComponent<CharacterMotor>();
	}
	
	// Update is called once per frame
	void Update () {
		if(holdingBall)
		{
			motor.canControl = false;
			if(Input.GetKeyDown(KeyCode.F))
			{
				
				//heldBall.rigidbody.velocity = transform.TransformDirection(new Vector3(0, 5, throwingForce));
			}
		}
		else
		{
			motor.canControl = true;
			if(Input.GetKeyDown(KeyCode.E))
			{
				for(int i = 0; i < balls.Length; i++)
				{
					if(Vector3.Distance(transform.position, balls[i].transform.position) < 10)
					{
						
						break;
					}
				}
			}
		}
	}
}
