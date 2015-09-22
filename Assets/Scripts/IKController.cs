using UnityEngine;
using System.Collections;

public class IKController : MonoBehaviour {

	public float ratio = 0.0f;

	bool isFire = false;
	Animator animator;
	Vector3 lookAtPosition;

	public void FireAnimation (Vector3 targetPosition) {
		isFire = true;
		lookAtPosition = targetPosition;
	}

	void OnAnimatorIK (int layerIndex) {
		animator.SetLookAtWeight (ratio);
		animator.SetLookAtPosition (lookAtPosition);
		animator.SetIKPositionWeight (AvatarIKGoal.RightHand, ratio);
		//animator.SetIKRotationWeight (AvatarIKGoal.RightHand, ratio);  
		animator.SetIKPosition (AvatarIKGoal.RightHand, lookAtPosition);
		//animator.SetIKRotation (AvatarIKGoal.RightHand, target.rotation);
	}

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (ratio > 1)
			isFire = false;
		ratio += isFire ? 0.25f : -0.1f;
		ratio = ratio < 0 ? 0 : ratio;
		//ratio = ratio > 1 ? 1 : ratio;
	}
}
