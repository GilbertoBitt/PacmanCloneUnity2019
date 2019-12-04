using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public GameState gameState;
    public int score = 0;
    public int playerLives = 0;
    public Text scoreText;
    public Canvas gameOverCanvas;
    public GhostMove[] ghostMoves;

    // Update is called once per frame
    void Update()
    {
        if (gameState == GameState.Dead && !gameOverCanvas.isActiveAndEnabled)
        {
            gameOverCanvas.enabled = true;
        }

        if (gameState == GameState.Dead && gameOverCanvas.isActiveAndEnabled)
        {
            if (Keyboard.current.anyKey.wasPressedThisFrame)
            {
                SceneManager.LoadScene(0, LoadSceneMode.Single);
            }
        }
    }

    public void AddScore(int addedScore)
    {
        score += addedScore;
        ScoreUpdated();
    }

    public void ScoreUpdated()
    {
        scoreText.text = score.ToString();
    }

    public void RunFromPacman()
    {
        foreach (var ghostMove in ghostMoves)
        {
            ghostMove.RunFromPacman();
        }
    }
   
}
