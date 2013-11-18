using UnityEngine;
using System.Collections;

public class TrailScript : MonoBehaviour {
	float trailTimer = 0;

	float maxTime = .05f;
	
	float dropTime = .5f;
	
	public GameObject trailPoint;
	
	GameObject[] trailArr;

	int maxPoints = 200;
	Vector3[] positions;
	int numPoints = 0;
	
	public bool isStealthWalking = false;
	
	LineRenderer trail;
	// Use this for initialization
	void Start () {
		trail = gameObject.GetComponentInChildren<LineRenderer>();
		positions = new Vector3[maxPoints];
		trailArr = new GameObject[maxPoints];
	}
	
	// Update is called once per frame
	void Update () {
		
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
			
			if(trailTimer >= dropTime)
			{
				trailTimer = 0;
				Instantiate(trailPoint, transform.position, Quaternion.identity);
				GameObject newPoint = (GameObject)Instantiate(trailPoint, transform.position, Quaternion.identity);
				InsertPoint(newPoint);
			}
		}
		
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
				Destroy(trailArr[i]);
				trailArr[i] = null;
				break;
			}
		}
	}
	
	void ResetTrail()
	{
		trail.SetVertexCount(0);
		numPoints = 0;
	}

}
