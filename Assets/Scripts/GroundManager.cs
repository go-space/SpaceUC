using UnityEngine;
using System.Collections;

public class GroundManager : SingletonMonoBehaviour<GroundManager> {

	private Material groundMaterial;
	private float offsetX;
	private float offsetZ;

	public void Scroll (float speedX, float speedZ) {
		offsetX += speedX * Time.deltaTime * -1;
		offsetZ += speedZ * Time.deltaTime * -1;
		offsetX = offsetX % 1;
		offsetZ = offsetZ % 1;
		groundMaterial.mainTextureOffset = new Vector2(offsetX, offsetZ);
	}

	void Awake ()
	{
		if (this != instance) {
			Destroy (this.gameObject);
			return;
		}
		DontDestroyOnLoad (this.gameObject);
	}

	// Use this for initialization
	void Start () {
		MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
		groundMaterial = meshRenderer.material;
	}

	// Update is called once per frame
	void Update () {
		
	}
}
