using UnityEngine;
using System.Collections;

public class PointCollectScript : MonoBehaviour {
	
	public bool isCollected  = false; 
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(isCollected)
		{
			gameObject.GetComponentInChildren<MeshRenderer>().material = Resources.Load("Materials/trail_stealth_mat") as Material;
		}
		else
		{
			gameObject.GetComponentInChildren<MeshRenderer>().material = Resources.Load("Materials/PointMat") as Material;
		}
	}
	
	void OnTriggerEnter(Collider coll)
	{
		GameObject obj = coll.gameObject;
		
		if(obj.GetComponent<TrailScript>() != null && !isCollected)
		{
			if(obj.GetComponent<NetworkView>().isMine && !obj.GetComponent<FPSInputControl>().isHunter)
			{
				obj.GetComponent<PlayerVarsScript>().points++;
				obj.GetComponent<PlayerGUIScript>().MakeUpdate("Point Collected");
				GameObject.Find ("GameGo").GetComponent<NetworkManager>().OnPointCollide();
				Debug.Log("Point Collected");
			}
			isCollected = true;
		}	
	}
}
