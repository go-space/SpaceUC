using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	/*
	void OnTriggerEnter (Collider other) {
		Debug.Log ("OnTriggerEnter");
	}
	*/

	void OnCollisionEnter (Collision other) {
		PlayerManager.instance.AddDamage (other.gameObject.tag);
	}
}
