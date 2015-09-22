using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public class TitleManager : SingletonMonoBehaviour<TitleManager> {

	/**
	 * タイトル画面管理クラス
	 */

	[SerializeField]
	private GameObject canvasPrefab;

	private GameObject canvas;
	private Text startLabel;

	/**
	 * スタートボタン押下
	 */
	private void OnClickStartButton () {
		// Debug.Log ("OnPushStartButton");
		AudioManager.instance.PlaySE ("Start");
		Application.LoadLevel("Game");
		/*
		UnityAction onClickAction = OnClickStartButton;
		canvas.transform.Find ("StartButton").GetComponent<Button> ().onClick.RemoveListener (onClickAction);
		*/
	}

	void Awake ()
	{
		// ボタンの参照取得とイベント登録
		UnityAction onClickAction = OnClickStartButton;
		canvas = (GameObject)Instantiate (canvasPrefab);
		canvas.transform.Find ("StartButton").GetComponent<Button> ().onClick.AddListener (onClickAction);
		startLabel = canvas.transform.Find ("StartLabel").GetComponent<Text> ();
	}

	// Use this for initialization
	void Start () {
		StartCoroutine (Blink());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/**
	 * メッセージの点滅
	 */
	IEnumerator Blink () {
		while (true) {
			startLabel.enabled = false;
			yield return new WaitForSeconds (1f);
			startLabel.enabled = true;
			yield return new WaitForSeconds (1f);
		}
	}
}
