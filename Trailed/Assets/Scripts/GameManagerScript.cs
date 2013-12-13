using UnityEngine;
using System.Collections;

public class GameManagerScript : MonoBehaviour {
	
	NetworkManager networkManager;
	
	public int POINTS_TO_WIN = 10;
	public int totalScore = 0;
	public bool gameOver = false;
	public bool hunterHasWon = false;
	
	private float pointSpawnTimer = 0;
	private float POINT_SPAWN_TIME = 20f;
	
	public GameObject[] pointsList;
	// Use this for initialization
	void Start () {
		networkManager = gameObject.GetComponent<NetworkManager>();
		
		pointsList = GameObject.FindGameObjectsWithTag("Point");
	}
	
	// Update is called once per frame
	void Update () {
		
		if(networkManager.isServer)
		{
			pointSpawnTimer += Time.deltaTime;
			
			if(pointSpawnTimer >= POINT_SPAWN_TIME)
			{
				pointSpawnTimer = 0;
				pointsList = GameObject.FindGameObjectsWithTag("Point");
				
				//To make sure the loop isn't infinite
				for(int i = 0; i < 20; i++)
				{
					int rand = Random.Range(0, pointsList.Length);
					if(pointsList[rand].GetComponent<PointCollectScript>().isCollected)
					{
						//pointsList[rand].GetComponent<PointCollectScript>().isCollected = false;
						networkManager.CreatePoint(rand);
						break;
					}
				}
			}
		}
		
		if(totalScore >= POINTS_TO_WIN)
		{
			gameOver = true;
			hunterHasWon = false;
		}
		if(gameOver)
		{
			networkManager.OnGameOver();
		}
	}
}
