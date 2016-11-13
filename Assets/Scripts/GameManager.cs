using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	public static GameManager Instance {
		private set;
		get;
	}

	GameManager() {
		Instance = this;
	}

	public Canvas gameOverCanvas;
	public GameObject gameOverBeatHighscore;
	public GameObject gameOverNotBeatHighscore;
	public Canvas hudCanvas;
	public Text distanceText;
	public Text highscoreText;

	public float TotalDistance {
		get;
		set;
	}

	public float HighScore {
		get;
		private set;
	}

	public bool IsGameOver {
		get;
		private set;
	}

	public void GameOver() {
		if (IsGameOver) {
			return;
		}

		IsGameOver = true;
		var hasBeatHighscore = TotalDistance > HighScore;
		if (hasBeatHighscore) {
			HighScore = TotalDistance;
			PlayerPrefs.SetFloat ("highscore", HighScore);
			PlayerPrefs.Save ();

			gameOverBeatHighscore.SetActive (true);
			gameOverNotBeatHighscore.SetActive (false);
		} else {
			gameOverBeatHighscore.SetActive (false);
			gameOverNotBeatHighscore.SetActive (true);
			
		}

		//hudCanvas.gameObject.SetActive (false);
		gameOverCanvas.gameObject.SetActive (true);
	}

	public void DeleteAllData() {
		HighScore = 0;
		PlayerPrefs.DeleteAll ();
		PlayerPrefs.Save ();
	}


	public void AddDistance(float dist) {
		if (IsGameOver) {
			return;
		}
		TotalDistance += dist;
	}

	public void StartNewGame() {
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}


	void Start() {
		HighScore = PlayerPrefs.GetFloat ("highscore");
		hudCanvas.gameObject.SetActive (true);
		gameOverCanvas.gameObject.SetActive (false);
		TotalDistance = 0;
		highscoreText.text = string.Format("{0:0.0} m", HighScore);
	}

	void Update() {
		distanceText.text = string.Format("{0:0.0} m", TotalDistance);
	}
}
