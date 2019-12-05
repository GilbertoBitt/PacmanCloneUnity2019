using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinkyTarget : MonoBehaviour
{
    public Vector3 targetPosition;
    public Transform pacmanTransform;
    public PlayerController playerController;
   

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3Int.RoundToInt(pacmanTransform.position) + ((Vector3) playerController.direction * 4);
    }
}
