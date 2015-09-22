using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	[SerializeField]
	protected int armor = 0;

	[SerializeField]
	protected string seName;

	[SerializeField]
	protected GameObject particlePrefab;

	[SerializeField]
	protected GameObject enemyBulletPrefab;

	[SerializeField]
	protected float bulletSpeed = 10f;

	protected Animator animator;
	protected Rigidbody rigidbody;
	protected Renderer[] rendererList;
	protected Collider[] colliderList;

	/**
	 * 敵の攻撃
	 * 敵の座標からプイレヤーの座標へ向けて弾を発射する
	 */
	public virtual void Fire () {
		GameObject bullet = (GameObject)Instantiate (enemyBulletPrefab, transform.position, Quaternion.identity);
		float distance = Vector3.Distance (PlayerManager.instance.PlayerCenterPosition, transform.position);
		Vector3 direction = (PlayerManager.instance.PlayerCenterPosition - transform.position).normalized;
		Vector3 force = direction * (distance + bulletSpeed);
		bullet.GetComponent<Rigidbody>().AddForce (force, ForceMode.Impulse);
		//bullet.transform.SetParent (transform.parent, false);

		// 発射後、2秒で消滅
		Destroy (bullet, 2f);
	}

	// Use this for initialization
	public void Start () {
		/**/
		animator = GetComponent<Animator> ();
		rigidbody = GetComponent<Rigidbody> ();
		rendererList = GetComponentsInChildren<Renderer> ();
		colliderList = GetComponentsInChildren<Collider> ();
		AudioManager.instance.PlaySE (seName);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter (Collision other) {
		if (other.gameObject.tag == "PlayerBullet") {
			armor--;
			if (armor < 0) {
				Kill ();
				//StartCoroutine (Kill ());
			} else {
				StartCoroutine (Damage ());
			}
		}
	}

	/*
	 * ダメージ演出
	 */
	IEnumerator Damage () {
		// AudioManager.instance.PlaySE ();

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
	void Kill () {
		AudioManager.instance.PlaySE ("Crash" + Random.Range(0, 2).ToString());

		animator.enabled = false;

		// 内包するコライダを無効
		foreach (Collider collider in colliderList) {
			//collider.enabled = false;
			//collider.isTrigger = true;
		}

		// 墜落させる
		rigidbody.useGravity = true;
		rigidbody.velocity = new Vector3 (0, 0, 0);

		Destroy (gameObject, 0.75f);

		PlayerManager.instance.AddPoint (gameObject.tag);
		
		gameObject.layer = LayerMask.NameToLayer ("Destroyed");

		GameObject particle = (GameObject) Instantiate (particlePrefab);
		particle.transform.SetParent (transform, false);

	}

	/**
	 * ダメージ演出（点滅）
	 * interval 点滅間隔（ミリ秒）
	 * count 点滅回数
	 */
	IEnumerator Blink (float interval = 0.1f, int count = 6) {
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
