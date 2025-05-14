using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuHighScoreDisplay : MonoBehaviour
{
    public TMP_Text highScoreText;

    void Start()
    {
        if (GameManager.instance != null)
        {
            highScoreText.text = "High Score\n" + GameManager.instance.highScore;
        }
    }
}

