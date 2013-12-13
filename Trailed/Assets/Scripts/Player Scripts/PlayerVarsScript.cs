using UnityEngine;
using System.Collections;

public class PlayerVarsScript : MonoBehaviour {
	
	private NetworkView view;
	public PlayerGUIScript playerGUI;
	
	public int points = 0;
	
	private string playerColor;
	public string PlayerColor { 
		get { return playerColor;}
		set { playerColor = value; }
	}
	private Material normalColor;
	
	public Material NormalColor {
		get {return normalColor;}
		set {normalColor = value; }
	}
	public Material StealthColor;// = Resources.Load("Materials/trail_stealth_mat") as Material;
	
	// Use this for initialization
	void Start () {
		StealthColor = Resources.Load("Materials/trail_stealth_mat") as Material;
		view = gameObject.GetComponent<NetworkView>();
		playerGUI = gameObject.GetComponent<PlayerGUIScript>();
	}
	
	// Update is called once per frame
	void Update () {
		if(view.isMine)
		{
			string message = "Your Score: " + points.ToString();
			playerGUI.SetPlayerScoreGUI(message);
		}
	}
}
