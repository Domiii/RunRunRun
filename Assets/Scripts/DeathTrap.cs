using UnityEngine;
using System.Collections;

public class DeathTrap : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		var player = other.GetComponent<Player> ();
		if (player != null) {
			// reset scene!
			GameManager.Instance.GameOver();
		}
	}
}
