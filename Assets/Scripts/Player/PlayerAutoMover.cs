using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAutoMover : MonoBehaviour
{
    [SerializeField] private float forwardSpeed = 3f;

    private void Update()
    {
        if (PlayerStatus.Instance.GetStatus(PlayerStatusType.IsGround))
        {
            transform.position += transform.forward * forwardSpeed * Time.deltaTime;
        }
    }
}