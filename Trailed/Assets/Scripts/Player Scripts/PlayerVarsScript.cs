using UnityEngine;
using System.Collections;

public class PlayerVarsScript : MonoBehaviour {
	
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
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
