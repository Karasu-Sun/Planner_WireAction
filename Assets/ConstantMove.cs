using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantMove : MonoBehaviour
{
    [SerializeField] private Vector3 moveDirection = Vector3.forward;
    [SerializeField] private float speed = 5f;

    private void Update()
    {
        transform.position += moveDirection.normalized * speed * Time.deltaTime;
    }
}