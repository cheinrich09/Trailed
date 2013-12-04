using UnityEngine;
using System.Collections;

public class PointScript : MonoBehaviour {
	
	public bool isStatic = false;
	public bool isDead = false;
	private float age = 0;
	private float lifetime = 10f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if(!isStatic)
		{
			age += Time.deltaTime;
		}
		
		if(age >= lifetime)
		{
			isDead = true;
			Destroy(gameObject);
		}
	}
}
