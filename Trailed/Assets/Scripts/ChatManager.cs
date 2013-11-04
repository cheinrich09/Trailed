using UnityEngine;
using System.Collections;

public class ChatManager : MonoBehaviour {

	private string messageToSend = "";
	private string chatPost = "";

	private Rect windowRect;
	private int windowTop;
	private int windowLeft = 10;
	private int windowWidth = 300;
	private int windowHeight = 140;
	private int padding = 20;
	private int textFieldHeight = 30;
	private int sendBtnWidth = 50;
	private Vector2 scrollPosition;
	private GUIStyle myStyle = new GUIStyle();
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void ChatWindow(int windowID)
	{
		scrollPosition = GUILayout.BeginScrollView (scrollPosition,
			GUILayout.Width(windowWidth - padding),
			GUILayout.Height(windowHeight - textFieldHeight*2));
		GUILayout.Label (chatPost, myStyle);
		GUILayout.EndScrollView();
		GUILayout.Space (5);
		GUILayout.BeginHorizontal();
			messageToSend = GUILayout.TextField (messageToSend,
				GUILayout.Width(windowWidth-sendBtnWidth-padding));
			if(GUILayout.Button ("Send", GUILayout.Width (sendBtnWidth)))
			{
				if(messageToSend != "")
				{
					GameObject nwm = GameObject.Find("GameGO");
					string playerName = nwm.GetComponent<NetworkManager>().PlayerName;
				
					if(Network.isClient == true)
					{	
						networkView.RPC("SendMessageToEveryone", RPCMode.All, messageToSend, playerName);
					}
					
					if(Network.isServer == true)
					{
						networkView.RPC ("SendMessageToEveryone", RPCMode.All, messageToSend, playerName+" (Server)");
					}
				}
				messageToSend="";
			}
		GUILayout.EndHorizontal();
	}
	
	[RPC]
	void SendMessageToEveryone (string mesage, string pName)
	{
		chatPost = pName +" : "+ messageToSend + "\n" + "\n" + chatPost;	
	}
	
	void OnGUI()
	{
		windowTop = Screen.height/5;
		windowRect = new Rect(windowLeft, windowTop, windowWidth, windowHeight);
		if(Network.peerType != NetworkPeerType.Disconnected)
		{
			//windowRect=ChatWindow(1);
			//windowRect = GUILayout.Window (0, windowRect, ChatWindow, "Chat");
		}
	}
}
