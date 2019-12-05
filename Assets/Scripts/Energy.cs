using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Energy : MonoBehaviour
{
    public GameManager gameManager;
    public bool hasBeenEaten = false;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.AddScore(50);
            gameManager.RunFromPacman();
            hasBeenEaten = true;
            gameObject.SetActive(false);
        }
    }
}
