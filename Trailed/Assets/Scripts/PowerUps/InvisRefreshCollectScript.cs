using UnityEngine;
using System.Collections;

public class InvisRefreshCollectScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider coll)
	{
		GameObject obj = coll.gameObject;
		
		if(obj.GetComponent<TrailScript>() != null)
		{
			TrailScript trailScript = obj.GetComponent<TrailScript>();
			trailScript.stealthTimer = 5f;
			trailScript.playerGUI.MakeUpdate("Stealth Refreshed");
			
			Debug.Log("Stealth Timer refreshed");
		}
	}
}
