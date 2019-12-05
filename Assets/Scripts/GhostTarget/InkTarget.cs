using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkTarget : MonoBehaviour
{
    public Vector3 targetPosition;
    public Transform pacmanTransform;
    public Transform blinkyTransform;
    public PlayerController playerController;
   

    // Update is called once per frame
    void Update()
    {
        Vector2 refPos = Vector2Int.RoundToInt(pacmanTransform.position) + (playerController.direction * 2);
        Vector2 blinkDeltaFromRefPos = Vector2Int.RoundToInt(blinkyTransform.position) - refPos;
        Vector2 invertDelta = blinkDeltaFromRefPos * - 1f;
        transform.position = refPos + invertDelta;
    }
}
