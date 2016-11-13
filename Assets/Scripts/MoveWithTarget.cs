using UnityEngine;
using System.Collections;

public class MoveWithTarget : MonoBehaviour {
	public Rigidbody target;
	public float turnSpeed = 2;

	Vector3 offset;

	void Start () {
		offset = transform.position - target.transform.position;
	}

	void Update () {
		if (target != null) {
			transform.position = target.transform.position + offset;
			var v = target.velocity;
			v.y = 0;
			if (v.sqrMagnitude > 0) {
				v.Normalize ();
				transform.forward = v;
			}
		}
	}
}
