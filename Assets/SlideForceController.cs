using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlideForceController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Rigidbody playerRb;

    [Header("Input")]
    [SerializeField] private InputActionReference leftStickAction;

    [Header("Force Points")]
    [SerializeField] private Transform RightForcePoint;
    [SerializeField] private Transform LeftForcePoint;

    [Header("Force Settings")]
    [SerializeField] private float forcePower = 10f;

    private void OnEnable()
    {
        leftStickAction.action.Enable();
    }

    private void OnDisable()
    {
        leftStickAction.action.Disable();
    }

    private void FixedUpdate()
    {
        Vector2 stick = leftStickAction.action.ReadValue<Vector2>();
        float x = stick.x;

        if (Mathf.Abs(x) < 0.1f)
            return; // 入力が小さい時

        if (x > 0) // スティック右
        {
            ApplyForce(RightForcePoint);
        }
        else if (x < 0) // スティック左
        {
            ApplyForce(LeftForcePoint);
        }
    }

    private void ApplyForce(Transform forcePoint)
    {
        Vector3 dir = (playerRb.position - forcePoint.position).normalized;
        playerRb.AddForce(dir * forcePower, ForceMode.Acceleration);
    }
}