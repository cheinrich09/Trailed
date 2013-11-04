using UnityEngine;
using System.Collections;

public class TrailScript : MonoBehaviour {
	float trailTimer = 0;
	float maxTime = .05f;
	int maxPoints = 200;
	Vector3[] positions;
	int numPoints = 0;
	
	public bool isStealthWalking = false;
	
	LineRenderer trail;
	// Use this for initialization
	void Start () {
		trail = gameObject.GetComponentInChildren<LineRenderer>();
		positions = new Vector3[maxPoints];
	}
	
	// Update is called once per frame
	void Update () {
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
	}
	
	void ResetTrail()
	{
		trail.SetVertexCount(0);
		numPoints = 0;
	}
}
