using UnityEngine;
using System.Collections;

public class EnemyBullet : MonoBehaviour {

	private Rigidbody rigidbody;

	// Use this for initialization
	void Start () {
		rigidbody = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		// 弾を回転させる
		rigidbody.AddTorque (Vector3.forward, ForceMode.Impulse);
	}
}
