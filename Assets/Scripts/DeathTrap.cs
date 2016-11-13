using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DeathTrap : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		var player = other.GetComponent<Player> ();
		if (player != null) {
			// reset scene!
			SceneManager.LoadScene (SceneManager.GetActiveScene().name);
		}
	}
}
