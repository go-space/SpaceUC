using UnityEngine;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;

public class PlayerManager : SingletonMonoBehaviour<PlayerManager>, IPointerDownHandler {

	/**
	 * プレイヤー管理クラス
	 */

	// 自動射撃・照準
	public bool isAutomatic;

	// プレイヤー
	public Transform playerTransform;

	public float speedX = 0;
	public float speedZ = 0;

	// 弾速
	public float bulletSpeed = 200f;

	// プレイヤーの移動範囲
	[SerializeField]
	private float playerLeft = -2.5f;
	[SerializeField]
	private float playerTop = 4.75f;
	[SerializeField]
	private float playerRight = 2.5f;
	[SerializeField]
	private float playerBottom = 0f;

	// カメラの移動最大値
	[SerializeField]
	private float cameraPositionTop = 5f;
	// カメラの移動最小値
	[SerializeField]
	private float cameraPositionBottom = 0.5f;
	// カメラのチルト最上値
	[SerializeField]
	private float cameraAngleTop = 370f;
	// カメラのチルト最下値
	[SerializeField]
	private float cameraAngleBottom = 340f;

	// 自動照準レンジ
	//[SerializeField]
	private Vector3 aimRange = new Vector3(0, 0, 5);

	// 銃口
	[SerializeField]
	private Transform gunMuzzle;

	// 弾丸のプレハブ
	[SerializeField]
	private GameObject bulletPrefab;

	private bool isActive;
	private bool isControl;

	private Collider collider;
	private Rigidbody rigidbody;

	private Animator unitychanAnimator;
	private IKController unitychanIKController;
	private Renderer[] unitychanRendererList;
	private Transform unitychanTransform;

	// コルーチン
	private Coroutine damageCoroutine;
	private Coroutine retireCoroutine;

	/**
	 * プレイヤーの中心座標
	 * （敵が攻撃する座標）
	 */
	public Vector3 PlayerCenterPosition {
		get {
			return playerTransform.position + new Vector3(0, 1, 0);
		}
	}
	
	public void OnPointerDown (PointerEventData eventData)
	{
		Debug.Log("OnPointerDown");
	}

	/**
	 * 弾丸の発射
	 */
	public void Shoot (Vector3 targetPosition) {
		Vector3 position = unitychanIKController.ratio > 0.5f ? gunMuzzle.position : new Vector3(playerTransform.position.x + 0.15f, playerTransform.position.y + 1.25f, -5.5f);
		Vector3 direction = (targetPosition - position).normalized;
		Vector3 force = direction * bulletSpeed;

		// 弾丸を生成し、クリックした方向へ向けて発射する
		GameObject bullet = (GameObject)Instantiate (bulletPrefab, position, Quaternion.identity);
		bullet.transform.LookAt (targetPosition);
		bullet.transform.SetParent (gameObject.transform, false);
		bullet.GetComponent<Rigidbody> ().AddForce (force, ForceMode.Impulse);

		// 発射後、2秒で消滅
		Destroy (bullet, 2f);

		unitychanIKController.FireAnimation (targetPosition);

		AudioManager.instance.PlaySE ("Shoot");
	}


	/**
	* GO破壊時にポイントを付与する
	* tag	破壊いたGOのタグ
	*/
	public int AddPoint (string tag) {

		DataManager dataManager = DataManager.instance;

		int point = dataManager.pointByTag.ContainsKey (tag) ? dataManager.pointByTag [tag] : 0;
		dataManager.Score += point;

		return point;
	}

	/**
	 * ダメージ追加
	 */
	public void AddDamage (string tag) {

		DataManager dataManager = DataManager.instance;

		if (!isActive || !dataManager.damageByTag.ContainsKey(tag))
			return;
		/*
		Debug.Log ("AddDamage > " + LayerMask.LayerToName (playerTransform.gameObject.layer));
		Debug.Log (collider.enabled);
		*/
		isActive = false;
		isControl = false;
		collider.enabled = false;

		//playerTransform.gameObject.layer = LayerMask.NameToLayer ("Destroyed");
		unitychanTransform.rotation = Quaternion.AngleAxis (0, Vector3.up);
		float damage = dataManager.damageByTag [tag];

		if (damageCoroutine != null)
			StopCoroutine(damageCoroutine);

		if (retireCoroutine != null)
			StopCoroutine(retireCoroutine);

		// ライフがある
		if (dataManager.Life > 0) {
			dataManager.SetLifeAtTime (dataManager.Life - damage);
			damageCoroutine = StartCoroutine (Damage());
			// ライフがない！
		} else {
			retireCoroutine = StartCoroutine (Retire());
		}
	}

	/*
	 * ダメージ演出
	 */
	IEnumerator Damage () {
		//Debug.Log ("ダメージ演出");

		isControl = true;
		unitychanAnimator.SetTrigger ("isDamage");
		AudioManager.instance.PlaySE ("Damage" + Random.Range(0, 2).ToString());

		yield return StartCoroutine(Blink());

		isActive = true;
		collider.enabled = true;
		playerTransform.rotation = Quaternion.identity;

		rigidbody.velocity = Vector3.zero;
		/*
		rigidbody.Sleep ();
		rigidbody.velocity = Vector3.zero;
		rigidbody.WakeUp ();
		*/
		//playerTransform.gameObject.layer = LayerMask.NameToLayer ("Player");
	}

	/*
	 * リタイア（ライフ０）演出
	 */
	IEnumerator Retire () {
		//Debug.Log ("リタイア演出");
		unitychanAnimator.SetTrigger ("isDown");
		AudioManager.instance.PlaySE ("Down");
		rigidbody.useGravity = true;
		rigidbody.velocity = Vector3.zero;
		rigidbody.Sleep ();

		yield return new WaitForSeconds (1);

		GameManager.instance.GamePose();
	}

	/*
	 * 復活（コンティニュー時など）
	 */
	public IEnumerator Recovery () {
		// Debug.Log ("Recovery");
		isControl = true;

		playerTransform.rotation = Quaternion.identity;
		unitychanAnimator.SetTrigger ("isRecovery");
		//AudioManager.instance.PlaySE (4);
		rigidbody.useGravity = false;
		rigidbody.velocity = Vector3.zero;

		yield return new WaitForSeconds (3);

		isActive = true;
		collider.enabled = true;
		rigidbody.WakeUp ();
		//playerTransform.gameObject.layer = LayerMask.NameToLayer ("Player");
	}

	/**
	 * ダメージ演出（点滅）
	 * interval 点滅間隔（ミリ秒）
	 * count 点滅回数
	 */
	IEnumerator Blink (float interval = 0.1f, int count = 8) {
		int counter = 0;
		while (counter < count) {
			counter++;
			foreach (Renderer renderer in unitychanRendererList) {
				renderer.enabled = !renderer.enabled;
			}
			yield return new WaitForSeconds (interval);
		}
		// Debug.Log ("点滅完了");
	}

	/**
	 * 自動照準
	 */
	IEnumerator AutoAim () {
		while (isAutomatic) {
			if (isControl) {
				Vector3 near = aimRange;
				bool isShoot = false;

				GameObject[] targetList = GameObject.FindGameObjectsWithTag ("Target");
				foreach (GameObject target in targetList) {
					if (target.transform.position.z > gunMuzzle.position.z && Vector3.Distance (target.transform.position, gunMuzzle.position) < Vector3.Distance (near, gunMuzzle.position)) {
						isShoot = true;
						near = target.transform.position;
					}
				}
				//isShoot = false;
				if (isShoot)
					Shoot (near);
				else
					Shoot (new Vector3 (gunMuzzle.position.x, gunMuzzle.position.y, gunMuzzle.position.z + 50));
			}
			yield return new WaitForSeconds (0.2f);
		}
	}

	void Awake () {
		collider = playerTransform.GetComponent<Collider>();
		rigidbody = playerTransform.GetComponent<Rigidbody>();

		unitychanTransform = playerTransform.Find ("unitychan");
		unitychanAnimator = unitychanTransform.GetComponent<Animator> ();
		unitychanIKController = unitychanTransform.GetComponent<IKController> ();
		unitychanRendererList = unitychanTransform.GetComponentsInChildren<Renderer> ();
	}

	// Use this for initialization
	void Start () {
		isControl = true;
		collider.enabled = false;
		StartCoroutine ("AutoAim");
		drangRange = Screen.dpi * 0.5f;
		//playerTransform.gameObject.layer = LayerMask.NameToLayer ("Destroyed");
	}
	
	private Vector3 beginPosition;
	private float centerX;
	private float centerY;
	private float drangRange;
	// Update is called once per frame
	void Update () {
		//Debug.Log(Screen.dpi + "/" + Screen.width);
		float angle;
		float ratio;
		float axisRawH = 0;
		float axisRawV = 0;
		float gameSpeed = DataManager.instance.gameSpeed;
		Vector3 delta = Vector3.zero;
		float deltaX = 0;
		float deltaY = 0;
		float moveX = 10f;
		float moveY = 10f;
		float ratioX = 0;
		float ratioY = 0;

		if (gameSpeed > 0)
			gameSpeed += DataManager.instance.stageIndex;

		if (Input.GetMouseButtonDown(0))
		{
			beginPosition = Input.mousePosition;
			centerX = Input.mousePosition.x;
			centerY = Input.mousePosition.y;
		}
			
		
		if (isControl) {
			if (Input.GetMouseButton(0))
			{
				delta = (Input.mousePosition - beginPosition).normalized;
				deltaX = Input.mousePosition.x - centerX;
				deltaY = Input.mousePosition.y - centerY;
				
				if (Mathf.Abs(deltaX) > drangRange)
				{
					centerX = Input.mousePosition.x > centerX ? Input.mousePosition.x - drangRange : Input.mousePosition.x + drangRange;
					deltaX = Input.mousePosition.x - centerX;
				}
				
				if (Mathf.Abs(deltaY) > drangRange)
				{
					centerY = Input.mousePosition.y > centerY ? Input.mousePosition.y - drangRange : Input.mousePosition.y + drangRange;
					deltaY = Input.mousePosition.y - centerY;
				}
				
				ratioX = deltaX / drangRange;
				ratioY = deltaY / drangRange;
				deltaX = (moveX * ratioX) * Time.deltaTime;
				deltaY = (moveY * ratioY) * Time.deltaTime;
			}			
				
			/*
			Debug.Log(Input.touchSupported);
			if (Input.touchSupported)
			{
				axisRawH = CrossPlatformInputManager.GetAxis("Horizontal");
				axisRawV = CrossPlatformInputManager.GetAxis ("Vertical");
			}
			else
			{
				axisRawH = Input.GetAxisRaw ("Horizontal");
				axisRawV = Input.GetAxisRaw ("Vertical");
			}
			*/
			axisRawH = deltaX;// * 0.1f * Time.deltaTime;;
			axisRawV = deltaY;// * 0.15f * Time.deltaTime;;
			/*
			axisRawH = axisRawH > 0 ? Mathf.Ceil(axisRawH) : Mathf.Floor(axisRawH);
			axisRawV = axisRawV > 0 ? Mathf.Ceil(axisRawV) : Mathf.Floor(axisRawV);
			axisRawH *= 0.1f;
			axisRawV *= 0.15f;
			*/
		}

		float x = axisRawH + playerTransform.position.x;
		float y = axisRawV + playerTransform.position.y;
		float z = playerTransform.position.z;

		// プレイヤー座標補正
		x = Mathf.Min (Mathf.Max (x, playerLeft), playerRight);
		y = Mathf.Min (Mathf.Max (y, playerBottom), playerTop);
		Vector3 position = new Vector3 (x, y, z);
		playerTransform.position = position;

		// カメラ位置とカメラチルト
		ratio = (playerTransform.position.y - playerBottom) / (playerTop - playerBottom);
		y = Mathf.Lerp (cameraPositionBottom, cameraPositionTop, ratio);
		Camera.main.transform.position = new Vector3 (Camera.main.transform.position.x, y, Camera.main.transform.position.z);
		angle = Mathf.Lerp (cameraAngleBottom, cameraAngleTop, ratio);
		Camera.main.transform.rotation = Quaternion.AngleAxis (angle, Vector3.right);

		// 地上・空中判定
		if (playerTransform.position.y > playerBottom) {
			unitychanAnimator.SetBool ("isFly", true);
			//unitychanTransform.rotation = Quaternion.AngleAxis (-45, Vector3.up);
		} else {
			unitychanAnimator.SetBool ("isFly", false);
			//unitychanTransform.rotation = Quaternion.AngleAxis (0, Vector3.up);
		}

		speedZ = gameSpeed;
		speedX = playerTransform.position.x / playerRight * gameSpeed * 1.5f;

		GroundManager.instance.Scroll (speedX, speedZ);
		ObstacleManager.instance.Scroll (speedX, speedZ);
	}
}
