using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkTarget : MonoBehaviour
{
    public Vector3 targetPosition;
    public Transform pacmanTransform;
   

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3Int.RoundToInt(pacmanTransform.position);
    }
}
