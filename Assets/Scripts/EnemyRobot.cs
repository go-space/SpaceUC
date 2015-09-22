using UnityEngine;
using System.Collections;

public class EnemyRobot : Enemy {

	private Transform robotKyle;
	private Transform wrist;
	private IKController ikController;

	// Use this for initialization
	public void Start () {
		base.Start ();
		robotKyle = transform.Find ("Robot Kyle");
		// 右手首の参照取得（弾の発射口）
		wrist = robotKyle.FindChild ("Root/Ribs/Right_Shoulder_Joint_01/Right_Upper_Arm_Joint_01/Right_Forearm_Joint_01/Right_Wrist_Joint_01");
		ikController = robotKyle.GetComponent<IKController>();
	}
		
	override public void Fire () {
		ikController.ratio = 0.5f;
		ikController.FireAnimation (PlayerManager.instance.PlayerCenterPosition);

		GameObject bullet = (GameObject)Instantiate (enemyBulletPrefab, wrist.transform.position, Quaternion.identity);
		float distance = Vector3.Distance (PlayerManager.instance.PlayerCenterPosition, wrist.transform.position);
		Vector3 direction = (PlayerManager.instance.PlayerCenterPosition - wrist.transform.position).normalized;
		Vector3 force = direction * (distance + bulletSpeed);
		bullet.GetComponent<Rigidbody>().AddForce (force, ForceMode.Impulse);
		//bullet.transform.SetParent (transform.parent, false);

		// 発射後、2秒で消滅
		Destroy (bullet, 2f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
