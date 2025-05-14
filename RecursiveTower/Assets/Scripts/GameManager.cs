using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public BoardManager boardScript;

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
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameSene")
        {
            Debug.Log("GameManager: Scene loaded, initializing board.");
            InitGame();
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

