using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour {

	[SerializeField]
	private int armor = 0;

	[SerializeField]
	private GameObject particlePrefab;

	private Renderer renderer;
	private Material material;
	private Color alpha = new Color(0, 0, 0, 0.1f);

	// Use this for initialization
	void Start () {
		/*
		gameObject.layer = LayerMask.NameToLayer ("Obstacle");
		gameObject.tag = "Obstacle";
		*/
		material = GetComponent<Renderer> ().material;
		renderer = GetComponent<Renderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.z < -15) {
			foreach (Transform child in gameObject.transform) {
				GameObject.Destroy(child.gameObject);
			}
			Destroy (gameObject);
		}
	}

	void OnCollisionEnter (Collision other) {
		if (other.gameObject.tag == "PlayerBullet") {
			armor--;
			if (armor < 0) {
				PlayerManager.instance.AddPoint (gameObject.tag);
				gameObject.layer = LayerMask.NameToLayer ("Destroyed");
				// SE?
				Instantiate (particlePrefab, other.gameObject.transform.position, Quaternion.identity);
				StartCoroutine (Blink ());
			}
		}
	}

	// ダメージ演出（点滅）
	IEnumerator Blink () {
		int count = 0;
		yield return new WaitForSeconds (0.25f);
		while (count < 6) {
			count++;
			renderer.enabled = !renderer.enabled;
			yield return new WaitForSeconds (0.1f);
		}
		Destroy (gameObject);
	}

	IEnumerator FadeOut () {
		while (material.color.a > 0) {
			material.color -= alpha;
			yield return true;
		}
		Destroy (gameObject);
	}
}
