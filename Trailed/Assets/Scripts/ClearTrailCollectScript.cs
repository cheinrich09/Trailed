using UnityEngine;
using System.Collections;

public class ClearTrailCollectScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider coll)
	{
		Debug.Log("This does work");
		GameObject obj = coll.gameObject;
		
		if(obj.GetComponent<TrailScript>() != null)
		{
			obj.GetComponent<TrailScript>().ResetTrail();
			Debug.Log("Trail Cleared");
		}
	}
}
