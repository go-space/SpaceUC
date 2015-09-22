using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : SingletonMonoBehaviour<AudioManager> {

	/**
	 * オーディオ管理クラス
	 */

	public float volume = 1;

	[System.Serializable]
	public class AudioClipData
	{
		public AudioClip clip;
		public string name;
		public float volume = 1.0f;
		public bool loop = true;

		public AudioClipData () {
			volume = 1.0f;
			loop = true;
		}
	}

	// BGM情報のリスト
	[SerializeField]
	private List <AudioClipData> bgmClipDataList;

	// SE情報のリスト
	[SerializeField]
	private List <AudioClipData> seClipDataList;

	// 名前からオーディオクリップを引く辞書
	private Dictionary <string, AudioClipData> bgmClipByName;
	private Dictionary <string, AudioClipData> seClipByName;

	// BGMのチャンネル
	private AudioSource bgmSource;

	// SEのチャンネルリスト(同時再生分用意)
	private AudioSource[] seSourceList = new AudioSource[8];

	/**
	 * BGM再生関数
	 * name	トラック名
	 * time		再生開始位置
	 */
	public float PlayBGM (string name, float time = 0) {
		if (!bgmClipByName.ContainsKey (name))
			return 0;

		AudioClipData clipData = bgmClipByName [name];
		bgmSource.clip = clipData.clip;
		bgmSource.loop = clipData.loop;
		bgmSource.time = time;
		bgmSource.volume = clipData.volume * volume;
		bgmSource.Play ();
		return clipData.clip.length - time;
	}

	/**
	 * SE再生関数
	 * name	トラック名
	 * time		再生開始位置
	 */
	public void PlaySE (string name, float time = 0) {
		if (!seClipByName.ContainsKey (name))
			return;

		AudioClipData clipData = seClipByName [name];

		// 空いているチャンネルがあれば再生
		foreach (AudioSource source in seSourceList) {
			if (!source.isPlaying) {
				source.clip = clipData.clip;
				source.loop = clipData.loop;
				source.time = time;
				source.volume = clipData.volume * volume;
				source.Play ();
				break;
			}
		}
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

		bgmClipByName = new Dictionary <string, AudioClipData> ();
		foreach (AudioClipData data in bgmClipDataList) {
			bgmClipByName [data.name] = data;
		}

		seClipByName = new Dictionary <string, AudioClipData> ();
		foreach (AudioClipData data in seClipDataList) {
			seClipByName [data.name] = data;
		}

		// BGM用のコンポーネント生成と参照取得
		bgmSource = gameObject.AddComponent<AudioSource> ();
		// bgmSource.loop = true;

		// SE用のコンポーネント生成と参照取得
		for (int i = 0; i < seSourceList.Length; i++) {
			seSourceList [i] = gameObject.AddComponent<AudioSource> ();
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
