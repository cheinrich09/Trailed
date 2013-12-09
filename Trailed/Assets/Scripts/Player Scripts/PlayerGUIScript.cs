using UnityEngine;
using System.Collections;

public class PlayerGUIScript : MonoBehaviour {
	
	private NetworkView view;
	
	public GUIText stealthGUI;
	public GUIText stealthTimerGUI;
	
	private float updatesTimer = 2.0f;
	private float updateLength = 2.0f;
	public GUIText updatesGUI;
	
	// Use this for initialization
	void Start () {
		
		view = gameObject.GetComponent<NetworkView>();
		
		stealthGUI = GameObject.Find("GUI_Stealth").GetComponent<GUIText>();
		stealthTimerGUI = GameObject.Find("GUI_StealthTimer").GetComponent<GUIText>();
		updatesGUI = GameObject.Find("GUI_Updates").GetComponent<GUIText>();
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
		}
	}
	
	public void MakeUpdate(string message)
	{
		updatesGUI.text = message;
		updatesTimer = 0;
	}
}
