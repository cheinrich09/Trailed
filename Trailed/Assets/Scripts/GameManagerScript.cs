using UnityEngine;
using System.Collections;

public class GameManagerScript : MonoBehaviour {
	public int POINTS_TO_WIN = 10;
	public int totalScore = 0;
	public bool gameOver = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(totalScore >= POINTS_TO_WIN)
		{
			gameOver = true;
		}
	}
}
