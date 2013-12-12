using UnityEngine;
using System.Collections;

public class PlayerGUIScript : MonoBehaviour {
	
	private NetworkView view;
	private GameManagerScript gameManager;
	
	private GUIText stealthGUI;
	private GUIText stealthTimerGUI;
	
	public void SetStealthGUI(string message)
	{
		if(stealthGUI == null)
		{
			Debug.Log("Wat?");
		}
		else
		{
			stealthGUI.text = message;
		}
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
	
	private float updatesTimer = 2.0f;
	private float updateLength = 2.0f;
	private GUIText updatesGUI;
	
	// Use this for initialization
	void Start () {
		
		view = gameObject.GetComponent<NetworkView>();
		
		gameManager = GameObject.Find("GameGo").GetComponent<GameManagerScript>();
		
		stealthGUI = GameObject.Find("GUI_Stealth").GetComponent<GUIText>();
		stealthTimerGUI = GameObject.Find("GUI_StealthTimer").GetComponent<GUIText>();
		updatesGUI = GameObject.Find("GUI_Updates").GetComponent<GUIText>();
		totalScoreGUI = GameObject.Find("GUI_TotalScore").GetComponent<GUIText>();
		playerScoreGUI = GameObject.Find("GUI_PlayerScore").GetComponent<GUIText>();
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
		}
	}
	
	public void MakeUpdate(string message)
	{
		updatesGUI.text = message;
		updatesTimer = 0;
	}
}
