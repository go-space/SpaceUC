using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public class GameManager : SingletonMonoBehaviour<GameManager> {

	/**
	 * ゲーム画面管理クラス
	 */

	[SerializeField]
	GameObject canvasPrefab;

	// UI
	private GameObject canvas;
	private GameObject stageUI;
	private GameObject continueUI;
	private Text scoreText;
	private Text highScoreText;
	private Text currentStageText;
	private Text stageText;
	private Text countDownText;
	private RectTransform lifeRectTransform;

	// コルーチン
	private Coroutine lifeBarScaleCoroutine;
	private Coroutine coundDownCoroutine;
	private Coroutine stageStartCoroutine;

	/*
	 * ゲーム開始
	 */
	public void GameStart () {
		// Debug.Log ("Game Start!!");
		AudioManager.instance.PlayBGM ("MainThema");
		StartCoroutine (LoopPlay(AudioManager.instance.PlayBGM ("MainThema")));
		DataManager.instance.stageIndex = 1;
		DataManager.instance.ResetScore ();
		GameReset ();
		stageStartCoroutine = StartCoroutine (StageStart ());
	}

	/*
	* ゲーム初期化
	*/
	public void GameReset () {
		// Debug.Log ("GameReset");
		continueUI.SetActive (false);
		DataManager.instance.gameSpeed = 6f;
		DataManager.instance.Life = 0;
		DataManager.instance.SetLifeAtTime (DataManager.instance.lifeMax, 1);
		StartCoroutine(PlayerManager.instance.Recovery ());
	}

	/*
	 * ゲーム中断
	 */
	public void GamePose () {
		// Debug.Log ("GamePose");
		DataManager.instance.gameSpeed = 0;
		EnemyManager.instance.StopScout ();
		continueUI.SetActive (true);
		if (coundDownCoroutine != null) {
			StopCoroutine (coundDownCoroutine);
		}

		coundDownCoroutine = StartCoroutine (CoundDown (10));
		//StopCoroutine (stageStartCoroutine);
	}

	/*
	 * ゲーム再開
	 */
	public void GameContinue () {
		// Debug.Log ("GameContinue");
		GameReset ();
		StopCoroutine (coundDownCoroutine);
		coundDownCoroutine = null;
		AudioManager.instance.PlaySE ("Start");
		EnemyManager.instance.StartScout ();
	}

	/*
	 * ゲーム終了
	 */
	public void GameOver () {

		// コールバック削除
		DataManager.instance.updateLifeCallback -= OnUpdateLife;
		DataManager.instance.updateScoreCallback -= OnUpdateScore;
		DataManager.instance.updateHighScoreCallback -= OnUpdateHighScore;

		// 結果画面（ランキング）へ遷移
		Application.LoadLevel ("Result");
	}

	/**
	 * ステージ開始
	 */
	IEnumerator StageStart () {
		stageUI.SetActive (true);
		currentStageText.text = DataManager.instance.stageIndex.ToString();
		stageText.text = DataManager.instance.stageIndex.ToString();

		yield return new WaitForSeconds (3);

		stageUI.SetActive (false);
		EnemyManager.instance.StartScout ();
	}

	/*
	 * ステージクリア
	 */
	public void StageClear () {
		DataManager.instance.stageIndex++;
		stageStartCoroutine = StartCoroutine (StageStart ());
	}

	/**
	 * ライフが更新されたら、UIへ反映させる
	 */
	private void OnUpdateLife (float life, float time) {
		if (lifeBarScaleCoroutine != null)
			StopCoroutine (lifeBarScaleCoroutine);

		lifeBarScaleCoroutine = StartCoroutine (LifeBarScale (life / DataManager.instance.lifeMax, time));
	}

	/**
	 * スコアが更新されたら、UIへ反映させる
	 */
	private void OnUpdateScore (int score) {
		scoreText.text = score.ToString ();
	}

	/**
	 * ハイスコアが更新されたら、UIへ反映させる
	 */
	private void OnUpdateHighScore (int highScore) {
		highScoreText.text = highScore.ToString ();
	}

	/**
	 * コンティニューボタン押下
	 */
	private void OnClickContinueButton () {
		// Debug.Log ("OnClickContinueButton");
		GameContinue ();
	}

	void Awake ()
	{
		// UIの参照取得
		canvas = (GameObject)Instantiate (canvasPrefab);
		scoreText = canvas.transform.Find ("ScoreText").GetComponent<Text> ();
		highScoreText = canvas.transform.Find ("HighScoreText").GetComponent<Text> ();
		currentStageText = canvas.transform.Find ("CurrentStageText").GetComponent<Text> ();
		lifeRectTransform = canvas.transform.Find ("LifeBar").GetComponent<RectTransform> ();

		stageUI = (GameObject)canvas.transform.Find ("StageUI").gameObject;
		stageText = stageUI.transform.Find ("StageText").GetComponent<Text> ();

		continueUI = (GameObject)canvas.transform.Find ("ContinueUI").gameObject;
		countDownText = continueUI.transform.Find ("CountDownText").GetComponent<Text> ();
		UnityAction onClickAction = OnClickContinueButton;
		continueUI.transform.Find ("ContinueButton").GetComponent<Button> ().onClick.AddListener (onClickAction);

		// コールバック登録
		DataManager.instance.updateLifeCallback += OnUpdateLife;
		DataManager.instance.updateScoreCallback += OnUpdateScore;
		DataManager.instance.updateHighScoreCallback += OnUpdateHighScore;
	}

	// Use this for initialization
	void Start () {
		GameStart ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/**
	 * ライフバーの伸縮
	 */
	IEnumerator LifeBarScale (float to, float tweenTime) {
		float elapsedTime = 0;
		float from = lifeRectTransform.localScale.x;
		float range = to - from;

		do {
			elapsedTime += Time.deltaTime;

			if (elapsedTime >= tweenTime)
				elapsedTime = tweenTime;

			float ratio = elapsedTime / tweenTime;
			float value = Mathf.Max (Mathf.Sin (Mathf.PI * 0.5f * ratio) * range + from, 0);

			lifeRectTransform.localScale = new Vector3 (value, 1, 1);

			yield return null;

		} while (elapsedTime < tweenTime);
	}

	/*
	* コンティニューカウントダウン
	*/
	IEnumerator CoundDown (int count) {
		while (count > -1) {
			countDownText.text = count.ToString();
			yield return new WaitForSeconds(1);
			count--;
		}
		GameOver ();
	}

	/**
	 * メインテーマのループ
	 */
	IEnumerator LoopPlay (float length) {
		yield return new WaitForSeconds (length);
		StartCoroutine (LoopPlay(AudioManager.instance.PlayBGM ("MainThema", 23.3f)));
	}

}
