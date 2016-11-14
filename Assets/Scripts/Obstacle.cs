using UnityEngine;
using System.Collections;


public class Obstacle : MonoBehaviour {
	public bool posXRandom;
	public bool posZRandom;
	public bool rotationRandom;
	public float speedX = 0;
	public float speedZ = 0;
	public float minDistance = 0;

	Vector3 localExtents;
	Bounds parentBounds;

	// the minimal x position that still keeps us inside the parent
	float LocalMinX {
		get { return -0.5f + localExtents.x; }
	}
	float LocalMaxX {
		get { return 0.5f - localExtents.x; }
	}
	float LocalMinZ {
		get { return -0.5f + localExtents.z; }
	}
	float LocalMaxZ {
		get { return 0.5f - localExtents.z; }
	}

	void Start() {
		ComputeExtends ();

		// setup!
		Invoke("DoSetup", 0.1f);
	}

	void ComputeExtends() {
		var mesh = GetComponent<MeshRenderer> ();
		if (mesh != null) {
			// single mesh
			localExtents = mesh.bounds.extents;
		}
		else {
			// multiple meshes!
			var meshes = GetComponentsInChildren<MeshRenderer> ();
			if (meshes != null && meshes.Length > 0) {
				var bounds = meshes [0].bounds;
				for (var i = 1; i < meshes.Length; ++i) {
					bounds.Expand(meshes [i].bounds.extents);
				}
				localExtents = bounds.extents;
			}
			else {
				Debug.LogError (GetType().Name +  " is missing MeshRenderer!", this);
			}
		}
	}

	void DoSetup() {
		// get parent and local information
		if (transform.parent != null) {
			var parentMesh = transform.parent.GetComponent<MeshRenderer> ();
			if (parentMesh != null) {
				parentBounds = parentMesh.bounds;
				localExtents.x /= parentBounds.extents.x;
				localExtents.y /= parentBounds.extents.y;
				localExtents.z /= parentBounds.extents.z;
				localExtents *= 0.5f;
			}
		}

		localExtents = transform.rotation * localExtents;

		InitPosition ();
	}

	void InitPosition() {
		var pos = transform.localPosition;
		if (posXRandom) {
			pos.x = Random.value > 0.5f ? LocalMinX : LocalMaxX;
		}
		if (posZRandom) {
			pos.z = Random.value > 0.5f ? LocalMinZ : LocalMaxZ;
		}
		transform.localPosition = pos;
	}

	void FixedUpdate () {
		// update position
		var pos = transform.localPosition;
		Move(ref pos.x, ref speedX, LocalMinX, LocalMaxX);
		Move(ref pos.z, ref speedZ, LocalMinZ, LocalMaxZ);
		transform.localPosition = pos;
	}

	void Move(ref float pos, ref float speed, float minVal, float maxVal) {
		if (speed != 0) {
			pos = Mathf.Clamp(pos + speed * Time.deltaTime, minVal, maxVal);
			if (pos <= minVal || pos >= maxVal) {
				// flip direction
				speed *= -1;
			}
		}
	}
}
