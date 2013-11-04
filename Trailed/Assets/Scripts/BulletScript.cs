using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour {

	public float BulletForce;// = 1;
	public GameObject parent;
	// Use this for initialization
	void Start () {
		//rigidbody.velocity = Vector3.zero;
		//rigidbody.AddForce(transform.forward*BulletForce);
	}
	
	// Update is called once per frame
	void Update () {
		//transform.Translate(transform.forward * (BulletSpeed*Time.deltaTime));
		Debug.DrawRay(transform.position, transform.forward);
		//if(
		if(transform.position.x > 50 || transform.position.x < -50 || transform.position.z >50 || transform.position.z < -50)
		{
			Destroy (gameObject);
		}
	}
	
	void OnCollisionEnter(Collision collision)
	{
		GameObject.Find ("GameGO").GetComponent<NetworkManager>().onBulletCollide(this.gameObject, collision);
	}
}
