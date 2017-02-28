using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshRenderer))]
public class Player : MonoBehaviour {
	public CollisionBuffer frontBuffer;
	public CollisionBuffer footBuffer;
	public float speed = 6.0f, maxSpeed = 16, acceleration = 1;
	public float turnSpeed = 3;
	public float strafeSpeed = 10;
	public float jumpPower = 8;
	public Vector3 Forward = Vector3.forward;


	Quaternion rightRotation;
	Rigidbody body;
	MeshRenderer mesh;
	int groundLayerMask;

	float horizontalMovement;
	Vector3 targetForward;
	Vector3 lastPos;

	GroundTile groundTile;
	Material groundTileMaterial;


	public GroundTile CurrentGroundTile {
		get {
			return groundTile;
		}
	}

	public bool IsOnGround {
		get { return footBuffer.colliders.Count > 1; }		// always collides with player
	}

	void Awake () {
		groundLayerMask = LayerMask.GetMask("Ground");
		Debug.Assert (groundLayerMask != 0);

		body = GetComponent<Rigidbody> ();
		mesh = GetComponent<MeshRenderer> ();
		targetForward = Forward;
//		leftRotation = Quaternion.Euler (0, -90, 0);
		rightRotation = Quaternion.Euler (0, 90, 0);
	}

	void Update () {
		UpdateGroundTile ();

		if (!GameManager.Instance.IsGameOver) {
			AddDistance ();
			Accelerate ();
			CheckJumpInput ();
			CheckRotationInput ();
		}
	}

	void FixedUpdate() {
		//UpdateGroundTile ();
		if (!GameManager.Instance.IsGameOver) {
			Turn ();
			Move ();
		}
	}

	void UpdateGroundTile() {
		RaycastHit hitInfo;
		var lastGroundTile = groundTile;
		var bounds = mesh.bounds;
		if (
			Physics.Raycast(transform.position, Vector3.down, out hitInfo, 100000, groundLayerMask) ||
			Physics.Raycast(bounds.max, Vector3.down, out hitInfo, 100000, groundLayerMask) ||
			Physics.Raycast(bounds.min, Vector3.down, out hitInfo, 100000, groundLayerMask)
		) {
			groundTile = hitInfo.collider.GetComponent<GroundTile> ();
			if (groundTile != null && groundTileMaterial == null) {
				groundTileMaterial = groundTile.GetComponent<Renderer> ().sharedMaterial;
			}
		}

		if (lastGroundTile != groundTile) {
			OnNewGroundTile (lastGroundTile);
		}
	}

	void OnNewGroundTile(GroundTile lastGroundTile) {
		// moved to new tile
		// update material
		if (groundTile != null) {
			var playerMaterial = GetComponent<Renderer> ().material;
			groundTile.GetComponent<Renderer> ().material.Lerp(groundTileMaterial, playerMaterial, 0.5f);
		}
		if (lastGroundTile != null) {
			// reset!
			lastGroundTile.GetComponent<Renderer> ().material = groundTileMaterial;
		}
	}

	/// <summary>
	/// Add to distance travelled
	/// </summary>
	void AddDistance() {
		var delta = transform.position - lastPos;
		// project movement delta onto forward direction (only add the distance travelled in forward direction)
		float dist;
		if (CurrentGroundTile != null) {
			dist = Vector3.Dot (delta, CurrentGroundTile.transform.forward);
		} else {
			delta.y = 0;
			dist = delta.magnitude;
		}
		GameManager.Instance.AddDistance (dist);
		lastPos = transform.position;
	}

	void Accelerate() {
		if (speed < maxSpeed) {
			speed = Mathf.Min (maxSpeed, speed + acceleration * Time.fixedDeltaTime);
		}
	}

	void CheckJumpInput() {
		if (IsOnGround && Input.GetAxisRaw ("Jump") > 0) {
			var v = body.velocity;
			v.y = jumpPower;
			body.velocity = v;
		}
	}

	void CheckRotationInput() {
		// check if player wants to turn
//		if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
//			targetForward = leftRotation * targetForward;
//		}
//		else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
//			targetForward = rightRotation * targetForward;
//		}
		horizontalMovement = Input.GetAxisRaw("Horizontal");
		var tile = CurrentGroundTile;
		if (tile != null && !Mathf.Approximately (tile.transform.rotation.eulerAngles.y, transform.root.eulerAngles.y)) {
			// turn in direction of tile
			targetForward = tile.transform.forward;
		}
	}

	void Turn() {
		// turn
		Forward = Vector3.RotateTowards(Forward, targetForward, turnSpeed * Time.fixedDeltaTime, 100000);
	}

	void Move() {
		var v = Forward * speed;
		v.y = body.velocity.y;
		body.velocity = v;

		// strafe (move horizontally)
		if (horizontalMovement != 0) {
			var right = rightRotation * Forward;
			right.y = 0;
			right.Normalize ();

			transform.position += right * horizontalMovement * strafeSpeed * Time.fixedDeltaTime;
		}
	}

	void OnCollisionEnter(Collision other) {
		if (frontBuffer != null && frontBuffer.colliders.Contains (other.collider)) {
			// we ran into something -> Dead!
			GameManager.Instance.GameOver();
		}
	}
}
