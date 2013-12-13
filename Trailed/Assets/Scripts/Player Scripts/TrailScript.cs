using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrailScript : MonoBehaviour {
	
	public PlayerGUIScript playerGUI;
	NetworkView view;
	
	//Dictionary<string, Material> mats;
	
	float trailTimer = 0;
	
	float dropTime = .5f;
	
	public float stealthTimer = 5f;
	
	public GameObject trailPoint;
	
	public bool isStealthWalking = false;
	
	// Use this for initialization
	void Start () {
		
		playerGUI = gameObject.GetComponent<PlayerGUIScript>();
		view = gameObject.GetComponent<NetworkView>();
		
		//mats = new Dictionary<string, Material>();
		//mats.Add("normal", Resources.Load("Materials/Blue") as Material);
		//mats.Add("stealth", Resources.Load("Materials/trail_stealth_mat") as Material);
	}
	
	// Update is called once per frame
	void Update () {
		
		//gameObject.GetComponentInChildren<MeshRenderer>().material = mats["normal"];
		
		
		if(view.isMine)
		{
		
			if(Input.GetKeyDown(KeyCode.R))
			{
				isStealthWalking = !isStealthWalking;
				if(stealthTimer <= 0)
				{
					isStealthWalking = false;
				}
			}
		
			trailTimer += Time.deltaTime;
		
			if(!isStealthWalking)
			{
				playerGUI.SetStealthGUI("");
				playerGUI.SetStealthTimerGUI("");
			
				gameObject.GetComponentInChildren<MeshRenderer>().material = gameObject.GetComponent<PlayerVarsScript>().NormalColor;
				//gameObject.GetComponentInChildren<MeshRenderer>().material = mats["stealth"];
				//gameObject.GetComponentInChildren<MeshRenderer>().material = gameObject.GetComponent<PlayerVarsScript>().StealthColor;

				if(trailTimer >= dropTime)
				{
					trailTimer = 0;
					Instantiate(trailPoint, transform.position, Quaternion.identity);
					GameObject newPoint = (GameObject)Instantiate(trailPoint, transform.position, Quaternion.identity);
					//InsertPoint(newPoint);
				}
			}
			else
			{
				if(stealthTimer > 0)
				{
					float roundedTimer = Mathf.Round(stealthTimer * 100f) / 100;
					string timerText = roundedTimer.ToString();
					playerGUI.SetStealthGUI("Hidden");
					playerGUI.SetStealthTimerGUI(timerText);
			
					stealthTimer -= Time.deltaTime;
					gameObject.GetComponentInChildren<MeshRenderer>().material = gameObject.GetComponent<PlayerVarsScript>().StealthColor;
					//gameObject.GetComponentInChildren<MeshRenderer>().material = mats["stealth"];
				}
				else
				{
					isStealthWalking = false;	
				}
			}
			
		}//End is mine

	}
}
