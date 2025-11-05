using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplePush : MonoBehaviour
{
    [Header("âüÇ∑ëŒè€")]
    [SerializeField] private Rigidbody targetRigidbody;

    [Header("óÕÇÃî≠ê∂à íu")]
    [SerializeField] private Transform pushOrigin;

    [Header("âüÇ∑óÕÇÃã≠Ç≥")]
    [SerializeField] private float pushForce = 10f;

    private bool wasRightGrapple = false;

    private void Update()
    {
        bool isRightGrapple = PlayerStatus.Instance.GetStatus(PlayerStatusType.IsRightGrapple);

        if (!wasRightGrapple && isRightGrapple)
        {
            ApplyPush();
        }

        wasRightGrapple = isRightGrapple;
    }

    private void ApplyPush()
    {
        if (targetRigidbody == null || pushOrigin == null) return;

        Vector3 direction = (targetRigidbody.position - pushOrigin.position).normalized;

        targetRigidbody.AddForce(direction * pushForce, ForceMode.Impulse);
    }
}