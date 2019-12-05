using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacDot : MonoBehaviour
{
    public GameManager gameManager;
    public bool hasBeenEaten = false;

    public void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.AddScore(10);
            hasBeenEaten = true;
            gameObject.SetActive(false);
        }
    }
}
