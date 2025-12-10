using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWall : MonoBehaviour
{
    [SerializeField] private MonoBehaviour targetScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Anchor"))
        {
            targetScript.enabled = false;
        }
    }
}