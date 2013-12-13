using UnityEngine;
using System.Collections;

public class PlayerGUIScript : MonoBehaviour {
	
	private NetworkView view;
	private GameManagerScript gameManager;
	
	private GUIText stealthGUI;
	private GUIText stealthTimerGUI;
	
	public void SetStealthGUI(string message)
	{
		stealthGUI.text = message;
	}
	
	public void SetStealthTimerGUI(string message)
	{
		stealthTimerGUI.text = message;
	}
	
	private GUIText totalScoreGUI;
	private GUIText playerScoreGUI;
	
	public void SetTotalScoreGUI(string message)
	{
		totalScoreGUI.text = message;
	}
	
	public void SetPlayerScoreGUI(string message)
	{
		playerScoreGUI.text = message;
	}
	
	private GUIText gameOverGUI;
	private GUIText winnerGUI;
	
	public void SetGameOverGUI(string message)
	{
		gameOverGUI.text = message;
	}
	
	public void SetWinnerGUI(string message)
	{
		winnerGUI.text = message;
	}
	
	private float updatesTimer = 2.0f;
	private float updateLength = 2.0f;
	private GUIText updatesGUI;
	
	// Use this for initialization
	void Awake () {
		
		view = gameObject.GetComponent<NetworkView>();
		
		gameManager = GameObject.Find("GameGo").GetComponent<GameManagerScript>();
		
		stealthGUI = GameObject.Find("GUI_Stealth").GetComponent<GUIText>();
		stealthTimerGUI = GameObject.Find("GUI_StealthTimer").GetComponent<GUIText>();
		updatesGUI = GameObject.Find("GUI_Updates").GetComponent<GUIText>();
		totalScoreGUI = GameObject.Find("GUI_TotalScore").GetComponent<GUIText>();
		playerScoreGUI = GameObject.Find("GUI_PlayerScore").GetComponent<GUIText>();
		gameOverGUI = GameObject.Find("GUI_GameOver").GetComponent<GUIText>();
		winnerGUI = GameObject.Find("GUI_Winner").GetComponent<GUIText>();
	}
	
	// Update is called once per frame
	void Update () {
		
		if(view.isMine)
		{
			if(updatesTimer < updateLength)
			{
				updatesTimer += Time.deltaTime;
			}
			else
			{
				updatesGUI.text = "";
			}
			
			totalScoreGUI.text = "Total Score: " + gameManager.totalScore;
			
			//Display Winner
			if(gameManager.gameOver)
			{
				gameOverGUI.text = "Game Over";
				
				if(gameManager.hunterHasWon && gameObject.GetComponent<FPSInputControl>().isHunter)
				{
					if(gameObject.GetComponent<FPSInputControl>().isHunter)
					{
						winnerGUI.color = Color.green;
						winnerGUI.text = "Winner";
					}
					else
					{
						winnerGUI.color = Color.red;
						winnerGUI.text = "You Lose";
					}
				}
				else
				{
					if(!gameObject.GetComponent<FPSInputControl>().isHunter)
					{
						winnerGUI.color = Color.green;
						winnerGUI.text = "Winner";
					}
					else
					{
						winnerGUI.color = Color.red;
						winnerGUI.text = "You Lose";
					}	
				}
			}
		}
	}
	
	public void MakeUpdate(string message)
	{
		updatesGUI.text = message;
		updatesTimer = 0;
	}
}
