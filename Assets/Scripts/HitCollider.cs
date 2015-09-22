using UnityEngine;
using System.Collections;

public class HitCollider : MonoBehaviour {

	[SerializeField]
	private GameObject mainBody;

	private BossDragon bossDragon;

	// Use this for initialization
	void Start () {
		bossDragon = mainBody.GetComponent<BossDragon> ();
	}

	void OnCollisionEnter (Collision other) {
		if (other.gameObject.tag == "PlayerBullet") {
			bossDragon.AddDamage ();
		}
	}
}
