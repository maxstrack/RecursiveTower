using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    private BoardManager boardScript;
	public int highScore = 0;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        boardScript = GetComponent<BoardManager>();

        // Subscribe to scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
		//highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
	    if (scene.name == "GameSene")
		{
			Debug.Log("GameManager: Scene loaded, initializing board.");

			// Find the new BoardManager in the freshly loaded scene
			boardScript = FindObjectOfType<BoardManager>();

			if (boardScript != null)
				InitGame();
			else
				Debug.LogWarning("GameManager: No BoardManager found in scene.");
		}
    }

	public void TryUpdateHighScore(int currentScore)
	{
		if (currentScore > highScore)
		{
			highScore = currentScore;
			PlayerPrefs.SetInt("HighScore", highScore);
			PlayerPrefs.Save();
			Debug.Log("New high score: " + highScore);
		}
	}

    void InitGame()
    {
        boardScript.SetupScene();
    }

    void OnDestroy()
    {
        // Always unsubscribe
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

