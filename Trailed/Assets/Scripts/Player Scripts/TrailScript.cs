using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrailScript : MonoBehaviour {
	
	public PlayerGUIScript playerGUI;
	NetworkView view;
	
	Dictionary<string, Material> mats;
	
	float trailTimer = 0;

	float maxTime = .05f;
	
	float dropTime = .5f;
	
	public float stealthTimer = 5f;
	
	public GameObject trailPoint;
	
	GameObject[] trailArr;

	int maxPoints = 10;
	Vector3[] positions;
	int numPoints = 0;
	
	public bool isStealthWalking = false;
	
	LineRenderer trail;
	// Use this for initialization
	void Start () {
		
		playerGUI = gameObject.GetComponent<PlayerGUIScript>();
		view = gameObject.GetComponent<NetworkView>();
		
		mats = new Dictionary<string, Material>();
		mats.Add("normal", Resources.Load("Materials/Blue") as Material);
		mats.Add("stealth", Resources.Load("Materials/trail_stealth_mat") as Material);
		
		trail = gameObject.GetComponentInChildren<LineRenderer>();
		positions = new Vector3[maxPoints];
		trailArr = new GameObject[maxPoints];
	}
	
	// Update is called once per frame
	void Update () {
		
		gameObject.GetComponentInChildren<MeshRenderer>().material = mats["normal"];
		
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
		
			for(int i = 0; i < trailArr.Length; i++)
			{
				if(trailArr[i] != null)
				{
					trailArr[i].GetComponent<PointScript>().isStatic = isStealthWalking;
					if(trailArr[i].GetComponent<PointScript>().isDead)
					{
						RemovePoint(trailArr[i]);
					}
				}
			}
		
			if(!isStealthWalking)
			{
				playerGUI.stealthGUI.text = "";
				playerGUI.stealthTimerGUI.text = "";
			
				if(trailTimer >= dropTime)
				{
					trailTimer = 0;
					Instantiate(trailPoint, transform.position, Quaternion.identity);
					GameObject newPoint = (GameObject)Instantiate(trailPoint, transform.position, Quaternion.identity);
					InsertPoint(newPoint);
				}
			}
			else
			{
				if(stealthTimer > 0)
				{
					float roundedTimer = Mathf.Round(stealthTimer * 100f) / 100;
					string timerText = roundedTimer.ToString();
					playerGUI.stealthGUI.text = "Hidden";
					playerGUI.stealthTimerGUI.text = timerText;
			
					stealthTimer -= Time.deltaTime;
					gameObject.GetComponentInChildren<MeshRenderer>().material = mats["stealth"];
				}
				else
				{
					isStealthWalking = false;	
				}
			}
			
		}//End is mine
		
		/*
		if(isStealthWalking)
		{
			ResetTrail();
		}
		trailTimer += Time.deltaTime;
		Debug.Log(trailTimer);
		Vector3 playerPos = trail.gameObject.transform.position;
		if(trailTimer >= maxTime)
		{
			trailTimer = 0;
			Vector3 newPos = new Vector3(playerPos.x, playerPos.y, playerPos.z);
			if(numPoints < maxPoints)
			{
				Debug.Log("Point Added");
				trail.SetVertexCount(numPoints + 1);
				trail.SetPosition(numPoints, newPos);
				positions[numPoints] = newPos;
				numPoints++;
			}
			else
			{
				Vector3[] temp = new Vector3[numPoints];
				for(int i = 0; i < numPoints - 1; i++)
				{
					positions[i] = positions[i + 1];
					
				}
				
				positions[numPoints - 1] = newPos;
				
				for(int i = 0; i < numPoints; i++)
				{
					trail.SetPosition(i, positions[i]);
				}
			}
		}
		*/

	}
	
	void InsertPoint(GameObject newPoint)
	{
		for(int i = 0; i < trailArr.Length; i++)
		{
			if(trailArr[i] == null)
			{
				trailArr[i] = newPoint;
				break;
			}
		}
	}
	
	public void RemovePoint(GameObject point)
	{
		for(int i = 0; i < trailArr.Length; i++)
		{
			if(trailArr[i] == point)
			{
				Debug.Log("PointRemoved");
				Destroy(trailArr[i]);
				trailArr[i] = null;
				break;
			}
		}
	}
	
	public void ResetTrail()
	{
		Debug.Log("Trail Reset");
		trail.SetVertexCount(0);
		numPoints = 0;
	}

}
