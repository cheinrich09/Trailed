using UnityEngine;
using System.Collections;
/*
//This camera smooths out rotation around the y-axis and height.
//Horizontal Distance to the target is always fixed.
//For every of those smoothed values we calculate the wanted value and the current value.
//Then we smooth it using the Lerp function.
//Then we apply the smoothed values to the transform's position.
*/
public class SmoothFollow : MonoBehaviour 
{
	public Transform target;
	public float distance = 2.0f;
	public float height = 2.0f;
	public float heightDamping = 2.0f;
	public float positionDamping = 2.0f;
	public float rotationDamping = 2.0f;
		
	// Update is called once per frame
	void LateUpdate ()
	{
		// Early out if we don't have a target
		if (!target)
			return;
		
		float dt = Time.deltaTime;
		float wantedHeight = target.position.y + height;
		float currentHeight = transform.position.y;
		
		// Damp the height
		//currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * dt);

		// Set the position of the camera 
		//Vector3 wantedPosition = target.position;// - target.forward * distance;
		//transform.position = wantedPosition;//Vector3.Lerp (transform.position, wantedPosition, positionDamping * dt);
	
		transform.position = target.position;
		// adjust the height of the camera
		//transform.position = new Vector3 (transform.position.x, currentHeight, transform.position.z);
		
		// look at the target

		//transform.forward = Vector3.Lerp (transform.forward, target.position - transform.position, rotationDamping * dt);
		transform.forward = Vector3.Lerp (transform.forward, target.forward, rotationDamping * dt);
		//transform.forward = new Vector3(transform.forward.x, transform.forward.y+Input.mousePosition.y, transform.forward.z);
		//transform.forward = Vector3.Lerp (transform.forward, new Vector3(target.forward.x, target.forward.y+Input.mousePosition.y, target.forward.z)-target.forward, rotationDamping * dt);
		//target.LookAt(Input.mousePosition);
		//transform.forward = Vector3.Lerp (transform.forward, target.forward, rotationDamping * dt);
		//transform.forward = target.forward;
		//transform.forward = target.forward;
		}
}