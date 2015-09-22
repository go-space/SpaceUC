using UnityEngine;
using System.Collections;

public class DestroyParticle : MonoBehaviour {

	private ParticleSystem particleSystem;

	void Start () {
		particleSystem = GetComponent<ParticleSystem> ();
	}
	void LateUpdate () {
		if (!particleSystem.IsAlive())
			Destroy (gameObject);
	}
}
