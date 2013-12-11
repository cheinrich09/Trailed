using UnityEngine;
using System.Collections;

public class SpawnPointScript : MonoBehaviour {
	
	public GameObject pointPrefab;
	//private GameObject[] spawnPoints;
	// Use this for initialization
	void Start () {
		//spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void CreatePoint()
	{
		Instantiate(pointPrefab, gameObject.transform.position, Quaternion.identity);	
	}
}
