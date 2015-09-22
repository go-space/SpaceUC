using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataManager : SingletonMonoBehaviour<DataManager> {

	/**
	 * データ管理クラス
	 */

	public float gameSpeed;

	// ライフ最大値
	public float lifeMax = 100;

	public int stageIndex;

	// スコアリスト
	public List<int> scoreList;

	public Dictionary <string, float> damageByTag;
	public Dictionary <string, int> pointByTag;

	// コールバック
	public delegate void UpdateLife(float life, float time = 0);
	public UpdateLife updateLifeCallback = null;
	public delegate void UpdateScore(int score);
	public UpdateScore updateScoreCallback = null;
	public delegate void UpdateHighScore(int highScore);
	public UpdateScore updateHighScoreCallback = null;

	[System.Serializable]
	public class DamageData {
		public string tagName;
		public float damageValue;
	}

	[System.Serializable]
	public class PointData {
		public string tagName;
		public int pointValue;
	}
	
	// ダメージ情報のリスト
	[SerializeField]
	private List<DamageData> damageDataList;

	// ポイント情報のリスト
	[SerializeField]
	private List<PointData> pointDataList;

	// ライフ
	[SerializeField]
	private float life;
	
	// スコア
	[SerializeField]
	private int score;

	[SerializeField]
	private int highScore;
	
	public float Life {
		get {
			return life;
		}
		set {
			life = value;
			if (updateLifeCallback != null)
				updateLifeCallback(life, 0);
		}
	}

	public void SetLifeAtTime (float value, float time = 0.25f) {
		life = value;
		updateLifeCallback(life, time);
	}

	public int Score {
		get {
			return score;
		}
		set {
			score = value;
			if (updateScoreCallback != null)
				updateScoreCallback(score);

			// スコアがハイスコアを上回ったら
			if (score > highScore) {
				HighScore = score;
			}
		}
	}

	public int HighScore {
		get {
			return highScore;
		}
		set {
			highScore = value;
			if (updateHighScoreCallback != null)
				updateHighScoreCallback(highScore);
		}
	}

	/**
	 * スコアの初期化
	 */
	public void ResetScore () {
		Score = 0;
		HighScore = highScore;
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

		// 初期のスコアランク生成
		highScore = 10000000;

		int i;
		int n = 20;
		int score = highScore;
		scoreList = new List<int> ();
		for (i = 0; i < n; i++) {
			scoreList.Add (score);
			score /= 2;
		}

		damageByTag = new Dictionary<string, float> ();
		foreach (DamageData data in damageDataList) {
			damageByTag [data.tagName] = data.damageValue;
		}

		pointByTag = new Dictionary<string, int> ();
		foreach (PointData data in pointDataList) {
			pointByTag [data.tagName] = data.pointValue;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
