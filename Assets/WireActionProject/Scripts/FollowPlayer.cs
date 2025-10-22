using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform target; // êePlayer
    void LateUpdate()
    {
        transform.position = target.position;
    }
}