using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour {
	public float speed = 6.0f, maxSpeed = 22, acceleration = 1;
	public float turnSpeed = 3;
	public float jumpPower = 8;
	public Vector3 Forward = Vector3.forward;


	Vector3 targetForward;
	Rigidbody body;
	int groundLayerMask, groundLayerIndex;
	Quaternion leftRotation, rightRotation;
	HashSet<GameObject> groundColliders;


	public GroundTile GetCurrentGroundTile() {
		RaycastHit hitInfo;
		if (Physics.Raycast (transform.position, Vector3.down, out hitInfo, 100000, groundLayerMask)) {
			var ground = hitInfo.collider.GetComponent<GroundTile>();
			return ground;
		}
		return null;
	}

	public bool IsOnGround {
		get { return groundColliders.Count > 0; }
	}

	void Awake () {
		groundLayerMask = LayerMask.GetMask("Ground");
		groundLayerIndex = LayerMask.NameToLayer ("Ground");
		Debug.Assert (groundLayerMask != 0);

		groundColliders = new HashSet<GameObject> ();
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

		// jump
		if (IsOnGround && Input.GetAxisRaw ("Jump") > 0) {
			body.velocity += Vector3.up * jumpPower;
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

	void OnCollisionEnter(Collision other) {
		if (other.gameObject.layer == groundLayerIndex && other.contacts[0].point.y < transform.position.y) {
			groundColliders.Add (other.gameObject);
		}
	}

	void OnCollisionExit(Collision other) {
		groundColliders.Remove(other.gameObject);
	}
}
