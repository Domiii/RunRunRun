using UnityEngine;
using System.Collections;

public class MoveWithTarget : MonoBehaviour {
	public Rigidbody target;
	public float turnSpeed = 8;

	Vector3 offset;
	Vector3 targetDirection;

	void Start () {
		offset = transform.position - target.transform.position;
		targetDirection = target.transform.forward;
	}

	void FixedUpdate () {
		if (target != null) {
			transform.position = target.transform.position + offset;
			var v = target.velocity;
			v.y = 0;
			if (v.sqrMagnitude > 0) {
				// cannot normalize zero vectors
				targetDirection = v;
				targetDirection.Normalize ();
			}
			transform.forward = Vector3.Slerp(transform.forward, targetDirection, Time.deltaTime * turnSpeed);
		}
	}
}
