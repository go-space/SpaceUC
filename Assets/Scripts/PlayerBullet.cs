using UnityEngine;
using System.Collections;

public class PlayerBullet : MonoBehaviour {

	void OnCollisionEnter (Collision other) {
		if (other.gameObject.tag == "Ground")
			Destroy (gameObject);
		else
			GetComponent<Collider> ().isTrigger = true;
	}

	void OnTriggerEnter (Collider other) {
		if (other.gameObject.tag == "Ground")
			Destroy (gameObject);
	}

	// Use this for initialization
	void Start () {
		//gameObject.layer = LayerMask.NameToLayer ("PlayerBullet");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
