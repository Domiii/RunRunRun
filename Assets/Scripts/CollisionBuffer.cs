using UnityEngine;
using System.Collections.Generic;

public class CollisionBuffer : MonoBehaviour {
	public HashSet<Collider> colliders {
		get;
		private set;
	}

	CollisionBuffer() {
		colliders = new HashSet<Collider> ();
	}

	void OnTriggerEnter(Collider other) {
		colliders.Add (other);
	}

	void OnTriggerExit(Collider other) {
		colliders.Remove(other);
	}
}
