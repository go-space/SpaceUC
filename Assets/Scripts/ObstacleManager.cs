using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObstacleManager : SingletonMonoBehaviour<ObstacleManager> {

	[SerializeField]
	GameObject obstaclePrefab;

	[SerializeField]
	private List <GameObject> obstaclePrefabList;

	private float spawnInterval = 5;
	private float spawnZ = 80f;

	private float offsetX;
	private float offsetZ;
	private float movement;

	/**
	 * 
	 */
	public void Scroll (float speedX, float speedZ) {
		float moveX = speedX * Time.deltaTime;
		float moveZ = speedZ * Time.deltaTime;

		offsetX += moveX * -1;
		offsetZ += moveZ * -1;
		/*
		offsetX = offsetX % 1;
		offsetZ = offsetZ % 1;
		*/
		GameObject[] obstacles = GameObject.FindGameObjectsWithTag ("Obstacle");
		foreach (GameObject obj in obstacles) {
			obj.transform.position += new Vector3 (moveX * -2.5f, 0, moveZ * -5f);
		}
		movement += moveZ;
		if (movement > spawnInterval) {
			movement = movement % spawnInterval;
			SpawnObstacle (speedX, speedZ);
		}
	}

	// 障害物の生成
	void SpawnObstacle (float speedX, float speedZ)
	{
		Vector3 position = new Vector3 (speedX * Random.Range(0.5f, 1), 0, speedZ * 2f).normalized;

		if (position != Vector3.zero)
			position *= (spawnZ / position.z);


		position += new Vector3 (PlayerManager.instance.playerTransform.position.x, 0, PlayerManager.instance.playerTransform.position.z);
		int index = Mathf.FloorToInt(Random.Range (0, obstaclePrefabList.Count));
		GameObject obstaclePrefab = obstaclePrefabList[index];

		GameObject obstacle = (GameObject)Instantiate (obstaclePrefab, position, Quaternion.identity);
		obstacle.transform.SetParent (gameObject.transform, false);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}
}
