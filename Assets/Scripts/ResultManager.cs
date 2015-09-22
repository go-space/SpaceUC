using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ResultManager : SingletonMonoBehaviour<ResultManager> {

	/**
	 * 結果画面管理クラス
	 */

	[SerializeField]
	private GameObject canvasPrefab;

	[SerializeField]
	private GameObject contentPrefab;

	private int playerIndex;
	private int rankMax = 20;
	private List<int> rankingList;

	private GameObject canvas;
	private Text highScoreText;
	private ScrollRect scrollRect;
	private RectTransform rectTransform;
	private Vector3 scrollPosition;

	/**
	 * リトライボタン押下
	 */
	private void OnClickRetryButton () {
		// Debug.Log ("OnClickRetryButton");
		AudioManager.instance.PlaySE ("Start");
		Application.LoadLevel("Game");
	}

	void Awake ()
	{
		// UIの参照取得
		canvas = (GameObject)Instantiate (canvasPrefab);

		RectTransform maskImage = (RectTransform)canvas.transform.Find ("MaskImage");
		RectTransform scrollContainer = (RectTransform)maskImage.Find ("ScrollContainer");
		scrollRect = scrollContainer.GetComponent<ScrollRect> ();
		rectTransform = (RectTransform)scrollContainer.Find ("LayoutContainer");

		RectTransform topScore = (RectTransform)canvas.transform.Find ("TopScore");
		highScoreText = topScore.transform.Find ("HighScoreText").GetComponent<Text> ();

		// ボタンの参照取得とイベント登録
		UnityAction onClickAction = OnClickRetryButton;
		canvas.transform.Find ("RetryButton").GetComponent<Button> ().onClick.AddListener (onClickAction);
	}

	// Use this for initialization
	void Start () {
		scrollRect.enabled = false;
		highScoreText.text = DataManager.instance.HighScore.ToString ();

		int i;
		int n = DataManager.instance.scoreList.Count;
		int rank = 1;

		playerIndex = -1;
		rankingList = new List<int> ();
		for (i = 0; i < n; i++) {
			int score = DataManager.instance.scoreList [i];
			if (playerIndex < 0 && DataManager.instance.Score > score) {
				playerIndex = rankingList.Count;
				rankingList.Add (DataManager.instance.Score);
				AddContent (rank, DataManager.instance.Score, true);
				rank++;
			}
			if (rankingList.Count < rankMax) {
				rankingList.Add (score);
				AddContent (rank, score);
				rank++;
			}
		}

		DataManager.instance.scoreList = rankingList;

		// RankIn
		if (playerIndex < 0) {
			Debug.Log ("ランクインせず");
			scrollPosition = rectTransform.localPosition;

			// Not RankIn
		} else {
			Debug.Log ("ランクインしました！");
			int topIndex = playerIndex - 2;
			topIndex = Mathf.Max (topIndex, 0);
			topIndex = Mathf.Min (topIndex, rankMax - 5);
			scrollPosition = rectTransform.localPosition + Vector3.up * topIndex * 60;
		}

		rectTransform.localPosition += Vector3.down * 300;
		StartCoroutine (AutoScroll());

		float length = AudioManager.instance.PlayBGM ("GameOver");
		StartCoroutine (GotoTitle (length));
	}
	
	// Update is called once per frame
	void Update () {
		GroundManager.instance.Scroll (0.1f, 0);
	}

	/**
	 * ランキングに追加
	 */
	void AddContent (int rank, int score, bool isPlayer = false) {
		GameObject content = (GameObject)Instantiate (contentPrefab);
		content.transform.SetParent (rectTransform.transform, false);
		Text rankText = content.transform.Find ("RankText").GetComponent<Text>();
		Text scoreText = content.transform.Find ("ScoreText").GetComponent<Text>();

		rankText.text = rank.ToString () + ".";
		scoreText.text = score.ToString ();

		// プレイヤのスコアは色を変える
		if (isPlayer) {
			Color color = new Color (255f / 255f, 178f / 255f, 210f / 255f, 1);
			rankText.color = color;
			scoreText.color = color;
		}
	}

	/*
	 * ランキングリストのスクロール
	 */
	IEnumerator AutoScroll () {
		while (!scrollRect.enabled) {
			if (rectTransform.localPosition.y < scrollPosition.y)
				rectTransform.localPosition += Vector3.up * 2;
			else {
				rectTransform.localPosition = scrollPosition;
				scrollRect.enabled = true;
			}
			yield return null;
		}
	}

	/**
	 * BGMのなり終わりでタイトル画面へ遷移
	 */
	IEnumerator GotoTitle (float time) {
		yield return new WaitForSeconds (time);
		Application.LoadLevel("Title");
	}
}
