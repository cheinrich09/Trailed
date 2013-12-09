using UnityEngine;
using System.Collections;

public class PointCollectScript : MonoBehaviour {

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
			if(obj.GetComponent<NetworkView>().isMine)
			{
				obj.GetComponent<PlayerVarsScript>().points++;
				obj.GetComponent<PlayerGUIScript>().MakeUpdate("Point Collected");
				Debug.Log("Point Collected");
			}
			Destroy(gameObject);
		}	
	}
}
