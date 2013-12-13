using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnScript : MonoBehaviour {

	/*[RequireComponent(typeof(CharacterMotor))]
	[AddComponentMenu("Character/FPS Input Control")]*/
	
	public GameObject PlayerPrefab;
	public GameObject BulletPrefab;
	public GameObject PointPrefab;
	public GameObject TrailPrefab;
	public GameObject[] spawnPoints;
	// Use this for initialization
	void Start () {
		spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
	}
	
	public GameObject spawnPlayer()
	{
		int index = Random.Range (0, spawnPoints.Length);
		GameObject sPoint = spawnPoints[index];
		//sPoint.GetComponent<PlayerLabel>().PlayerName = sPoint;
		//Network.Instantiate(PlayerPrefab,
		//	sPoint.transform.position, Quaternion.identity, 0).GetComponent<PlayerLabel>().PlayerName;
		//GameObject GO =
		return (GameObject)Network.Instantiate(PlayerPrefab,
			sPoint.transform.position, Quaternion.identity, 0);
		
	}
	
	public GameObject spawnBullet(GameObject SpawnPoint, GameObject Shooter)
	{
		return (GameObject)Network.Instantiate(BulletPrefab,
			SpawnPoint.transform.position, Shooter.transform.rotation, 0);

	}
	
	public GameObject spawnPoint(GameObject location)
	{
		return (GameObject)Network.Instantiate(PointPrefab, 
			location.transform.position, Quaternion.identity, 0);
	}
	
	public GameObject spawnTrail(GameObject location)
	{
		return (GameObject)Network.Instantiate(TrailPrefab, 
			location.transform.position, Quaternion.identity, 0);
	}
	// Update is called once per frame
	void Update () {
	
	}
	

}
