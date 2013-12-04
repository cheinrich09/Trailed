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
		Debug.Log("Point Collected");	
	}
}
