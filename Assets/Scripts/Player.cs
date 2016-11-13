using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour {
	public float speed = 6.0f, maxSpeed = 22, acceleration = 1;
	public float turnSpeed = 3;
	public Vector3 Forward = Vector3.forward;


	Vector3 targetForward;
	Rigidbody body;
	int groundLayer;
	Quaternion leftRotation, rightRotation;


	public GroundTile GetCurrentGroundTile() {
		RaycastHit hitInfo;
		if (Physics.Raycast (transform.position, Vector3.down, out hitInfo, 100000, groundLayer)) {
			var ground = hitInfo.collider.GetComponent<GroundTile>();
			return ground;
		}
		return null;
	}

	void Awake () {
		groundLayer = LayerMask.GetMask("Ground");
		Debug.Assert (groundLayer != 0);

		body = GetComponent<Rigidbody> ();
		targetForward = Forward;
		leftRotation = Quaternion.Euler (0, -90, 0);
		rightRotation = Quaternion.Euler (0, 90, 0);
	}

	void Update () {
		// accelerate
		if (speed < maxSpeed) {
			speed = Mathf.Min (maxSpeed, speed + acceleration * Time.fixedDeltaTime);
		}

		// update direction of rotation
		UpdateRotation ();
	}

	void FixedUpdate() {
		// keep turning
		KeepTurning();

		// keep going!
		var v = Forward * speed;
		v.y = body.velocity.y;

		body.velocity = v;
	}

	void UpdateRotation() {
		// check if player wants to turn
		if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
			targetForward = leftRotation * targetForward;
		}
		else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
			targetForward = rightRotation * targetForward;
		}
	}

	void KeepTurning() {
		Forward = Vector3.RotateTowards(Forward, targetForward, turnSpeed * Time.fixedDeltaTime, 100000);
	}
}
