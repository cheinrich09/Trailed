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
			obj.GetComponent<TrailScript>().stealthTimer = 3f;
			Debug.Log("Stealth refreshed");
		}
	}
}
