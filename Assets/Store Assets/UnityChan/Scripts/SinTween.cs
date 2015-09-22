using UnityEngine;
using System.Collections;

public class SinTween : MonoBehaviour {

	float startTime;
	float from = 200f;
	float to = 100f;
	float time = 0.5f;
	float elapsedTime;

	// Use this for initialization
	void Start () {
		startTime = Time.time;
		elapsedTime = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButton("Fire1")) {

			elapsedTime += Time.deltaTime;
			if (elapsedTime >= time) {
				elapsedTime = time;
				//Debug.Log ("complete");
			}

			float ratio = elapsedTime / time;
			float range = to - from;
			float value = Mathf.Sin (Mathf.PI * 0.5f * ratio) * range + from;
			Debug.Log (value);
			float scaleX = Mathf.Sin (Mathf.PI * 0.5f * ratio);

			gameObject.transform.localScale = new Vector3(scaleX, 1, 1);
		}
	}
}
