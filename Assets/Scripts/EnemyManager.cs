using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : SingletonMonoBehaviour<EnemyManager> {

	/**
	 * エネミー（敵キャラ）管理クラス
	 */

	[SerializeField]
	private List <EnemyFormationData> formationList;

	/**
	 * エネミーの編隊データ
	 * エネミーデータと出現タイミング
	 */
	[System.Serializable]
	public class EnemyFormationData {
		public List <EnemyData> enemyPrefabList;
		public float interval;
	}

	/**
	 * エネミーデータ
	 * フレハブとアニメーション名
	 */
	[System.Serializable]
	public class EnemyData {
		public GameObject enemyPrefab;
		public string stateName;
	}

	private int enemyIndex;

	// コルーチン
	private Coroutine scoutEnemyCoroutine;

	public void StartScout () {
		coroutineFlag = true;
		if (scoutEnemyCoroutine != null) {
			StopCoroutine (scoutEnemyCoroutine);
		}

		scoutEnemyCoroutine = StartCoroutine (ScoutEnemy());
	}

	public void StopScout () {
		coroutineFlag = false;
		StopCoroutine (scoutEnemyCoroutine);
		scoutEnemyCoroutine = null;
	}
	
	// Use this for initialization
	void Start () {
		enemyIndex = 0;
		//enemyIndex = formationList.Count - 3;
	}
	
	// Update is called once per frame
	void Update () {

	}

	private bool coroutineFlag;

	/**
	 * エネミーの召喚
	 */
	IEnumerator ScoutEnemy () {
		while (true) {
			GameObject[] targetList = GameObject.FindGameObjectsWithTag ("Target");
			if (targetList.Length < 1) {
				StartCoroutine (GenerateEnemy ());
			} else {
				//Debug.Log(targetList.Length + "体の敵がいます");
			}

			if (coroutineFlag)
				yield return null;
			else
				yield break;
		}
	}

	/**
	 * エネミーの生成
	 */
	IEnumerator GenerateEnemy () {
		EnemyFormationData formationData = formationList [enemyIndex];
		int n = formationData.enemyPrefabList.Count;
		for (int i = 0; i < n; i++) {
			EnemyData enemyData = formationData.enemyPrefabList [i];
			GameObject enemy = (GameObject)Instantiate (enemyData.enemyPrefab);
			enemy.transform.SetParent (transform, true);
			Animator animator = enemy.GetComponent<Animator> ();
			animator.Play (enemyData.stateName);

			if (formationData.interval > 0)
				yield return new WaitForSeconds (formationData.interval);
		}

		if (enemyIndex < formationList.Count - 1)
			enemyIndex++;
		else
			enemyIndex = 0;
			
		//enemyIndex = formationList.Count - 2;
	}
}
