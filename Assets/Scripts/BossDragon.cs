using UnityEngine;
using System.Collections;

public class BossDragon : MonoBehaviour {

	// 耐久値
	[SerializeField]
	private int armor = 0;

	// 銃口（炎を射出する座標）
	[SerializeField]
	private Transform mouth;

	// 弾丸（炎）
	[SerializeField]
	private GameObject bulletPrefab;

	[SerializeField]
	private GameObject particlePrefab;

	private Animation animation;
	private Animator animator;
	private float bulletSpeed = 5f;
	private string stateName = "";
	private Renderer[] rendererList;
	private Collider[] colliderList;

	/**
	 * ボスの攻撃
	 * ボスの口座標からプイレヤーの座標へ向けて炎を発射する
	 */
	public void Fire () {
		GameObject bullet = (GameObject)Instantiate (bulletPrefab, mouth.position, Quaternion.identity);
		bullet.transform.rotation = Quaternion.LookRotation (PlayerManager.instance.PlayerCenterPosition - mouth.position);

		animation.Blend ("bite", 0.75f);

		// 発射後、2秒で消滅
		Destroy (bullet, 10f);
	}

	public void AddDamage () {
		armor--;
		if (armor < 0) {
			StartCoroutine (Kill ());
		} else {
			StartCoroutine (Damage ());
		}
	}

	public void ChangeState (string value) {
		stateName = value;
		animation.CrossFade (stateName, 1f);
	}

	// Use this for initialization
	void Start () {
		animation = GetComponentInChildren<Animation>();
		animator = GetComponentInChildren<Animator>();
		rendererList = GetComponentsInChildren<Renderer> ();
		colliderList = GetComponentsInChildren<Collider> ();

		string[] array = {
			"flyForward",
			"flyLeft",
			"flyRight",
			//"bite"
		};

		// アニメーションスピード調整
		foreach (string stateName in array) {
			//Debug.Log (stateName);
			AnimationState animationState = animation [stateName];
			animationState.speed = 0.5f;
		}
		/*
		AnimationState flyForwardState = animation ["flyForward"];
		flyForwardState.speed = 0.25f;
		foreach (AnimationState state in animation) {
			Debug.Log (state.name);
			state.speed = 0.25f;
		}
		*/
	}

	/*
	 * ダメージ演出
	 */
	IEnumerator Damage () {
		AudioManager.instance.PlaySE ("BossDamage");

		// 内包するコライダを無効
		foreach (Collider collider in colliderList) {
			collider.enabled = false;
		}

		// 点滅完了を待つ
		yield return StartCoroutine (Blink ());

		// 内包するコライダを有効
		foreach (Collider collider in colliderList) {
			collider.enabled = true;
		}
	}

	/*
	 * 討伐演出
	 */
	IEnumerator Kill () {
		AudioManager.instance.PlaySE ("BossDie");

		animation.CrossFade ("death", 0.75f);
		animator.enabled = false;

		transform.position = new Vector3(transform.position.x, 0, transform.position.z);

		// 内包するコライダを無効
		foreach (Collider collider in colliderList) {
			collider.enabled = false;
		}

		// アニメーション完了を待つ
		yield return new WaitForSeconds (2.233f);

		// 点滅完了を待つ
		yield return StartCoroutine (Blink (0.1f, 5));

		Destroy (gameObject);
		/*
		if (ScoreManager.instance.pointByTag.ContainsKey (gameObject.tag)) {
			int point = ScoreManager.instance.pointByTag [gameObject.tag];
			DataManager.instance.Score += point;
		}
		*/
		PlayerManager.instance.AddPoint (gameObject.tag);
		GameManager.instance.StageClear ();
	}

	/**
	 * ダメージ演出（点滅）
	 * interval 点滅間隔（ミリ秒）
	 * count 点滅回数
	 */
	IEnumerator Blink (float interval = 0.05f, int count = 6) {
		int counter = 0;
		while (counter < count) {
			counter++;
			foreach (Renderer renderer in rendererList) {
				renderer.enabled = !renderer.enabled;
			}
			yield return new WaitForSeconds (interval);
		}
	}

}
