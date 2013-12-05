using UnityEngine;
using System.Collections;

public class GameTimerScript : MonoBehaviour {
	
	private float GAME_LENGTH = 60;
	private float floatTimer = 60;
	public int gameTimer = 60;
	public bool gameOver = false;
	public bool start = false;
	
	private GUIText timerGUI;
	// Use this for initialization
	void Start () {
		timerGUI = GameObject.Find("GUI_GameTimer").GetComponent<GUIText>();
		StartGame ();
	}
	
	// Update is called once per frame
	void Update () {
		if(start)
		{
			if(!gameOver)
			{
				floatTimer -= Time.deltaTime;
				gameTimer = (int)floatTimer;
				timerGUI.text = "Time Remaining: " + gameTimer;
		
				if(gameTimer <= 0)
				{
					gameTimer = -1;
					gameOver = true;
				}
			}
			else
			{
				timerGUI.text = "Game Over";
			}
		}
	}
	
	void StartGame()
	{
		start = true;
	}
}
