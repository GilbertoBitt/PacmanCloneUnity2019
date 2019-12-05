using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public GameState gameState;
    public int score = 0;
    public int playerLives = 3;
    public Text scoreText;
    public Canvas gameOverCanvas;
    public Canvas winCanvas;
    public GhostMove[] ghostMoves;
    public Transform text200Points;
    public Image[] lifeDisplayItem;
    public PlayerController pacmanController;
    private float _timeToChangeMode;
    private float _timeToExitGhostMode;
    public int pacdotsRemaining = 80;
    public PacDot[] pacDots;
    public Energy[] energies;
    public bool isDone = false;
    public bool AllPacdotsEatern => pacDots.All(x => x.hasBeenEaten) && energies.All(x => x.hasBeenEaten);
 
    public void Start()
    {
        pacDots = FindObjectsOfType<PacDot>();
        energies = FindObjectsOfType<Energy>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState == GameState.Dead && !gameOverCanvas.isActiveAndEnabled && playerLives <= 0)
        {
            gameOverCanvas.enabled = true;
        }
        
        if (gameState == GameState.Dead && gameOverCanvas.isActiveAndEnabled)
        {
            if (Keyboard.current.anyKey.wasPressedThisFrame)
            {
                SceneManager.LoadScene("game", LoadSceneMode.Single);
            }
        }

        if (Time.time >= _timeToChangeMode)
        {
            _timeToChangeMode = Time.time + 5f;
            foreach (var ghostMove in ghostMoves)
            {
                if (ghostMove.state == GhostMove.State.Chase)
                {
                    ghostMove.state = GhostMove.State.Scatter;
                } else if (ghostMove.state == GhostMove.State.Scatter)
                {
                    ghostMove.state = GhostMove.State.Chase;
                }
            }
        }

        if (isDone && !gameOverCanvas.isActiveAndEnabled)
        {
            winCanvas.enabled = true;
        }

        if (isDone && gameOverCanvas.isActiveAndEnabled)
        {
            if (Keyboard.current.anyKey.wasPressedThisFrame)
            {
                SceneManager.LoadScene("game", LoadSceneMode.Single);
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
        scoreText.text = $"Score \n {score}";
    }

    public void RunFromPacman()
    {
        foreach (var ghostMove in ghostMoves)
        {
            ghostMove.RunFromPacman();
        }
    }

    public void UpdateDisplayLife()
    {
        for (int i = 0; i < 3; i++)
        {
            lifeDisplayItem[i].enabled = i+1 <= playerLives;
        }
    }
    
    public void ResetAllGhost()
    {
        foreach (var ghostMove in ghostMoves)
        {
            ghostMove.ResetGhost();
        }
    }

    public void CatchGhostScore(Vector3 onPosition)
    {
        score += 200;
        ScoreUpdated();
        text200Points.position = Vector3Int.RoundToInt(onPosition);
        StartCoroutine(DelayedCallback(1f, () =>
        {
            text200Points.position = Vector3.one * 200f; 
        }));
    }

    public void GhostCatchPacman()
    {
        if (playerLives > 0)
        {
            playerLives--;
            gameState = GameState.Reviving;
            UpdateDisplayLife();
            ResetAllGhost();
            StartCoroutine(pacmanController.PlayDeadAnimation());
            StartCoroutine(DelayedCallback(1f, () =>
            {
                gameState = GameState.Running;
            }));
        }
        else
        {
            gameState = GameState.Dead;
            UpdateDisplayLife();
        }
        
    }
    
    public IEnumerator DelayedCallback(float delay, Action callBack) {
        // Waits 3 seconds
        yield return new WaitForSeconds(delay);
        // CALL callBack after 3 seconds
        callBack();
    }
   
}
