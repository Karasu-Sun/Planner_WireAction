using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushFromBehind : MonoBehaviour
{
    [Header("対象")]
    public Rigidbody targetRigidbody;

    [Header("力設定")]
    public float pushForce = 10f;

    [Header("方向設定")]
    public Transform behindTransform;

    private void Start()
    {
        if (targetRigidbody == null)
            targetRigidbody = GetComponent<Rigidbody>();
    }

    [Header("グラップル解除後の余韻時間")]
    public float pushAfterGrappleTime = 0.3f;
    private float pushTimer = 0f;

    private void Update()
    {
        bool isGrappling =
            PlayerStatus.Instance.GetStatus(PlayerStatusType.IsGrappling) ||
            PlayerStatus.Instance.GetStatus(PlayerStatusType.IsRightGrapple) ||
            PlayerStatus.Instance.GetStatus(PlayerStatusType.IsLeftGrapple);

        if (isGrappling)
        {
            pushTimer = pushAfterGrappleTime;
            Push();
        }
        else if (pushTimer > 0f)
        {
            pushTimer -= Time.deltaTime;
            SwingPush();
        }
    }

    public void Push()
    {
        if (targetRigidbody == null || behindTransform == null) return;

        Vector3 pushDir = (targetRigidbody.position - behindTransform.position).normalized;

        targetRigidbody.AddForce(pushDir * pushForce, ForceMode.Impulse);
    }

    private float swingFoceDistance = 200f; // スイング時に発生している慣性エネルギーの割合

    public void SwingPush()
    {
        if (targetRigidbody == null || behindTransform == null) return;

        Vector3 pushDir = (targetRigidbody.position - behindTransform.position).normalized;

        targetRigidbody.AddForce(pushDir * pushForce * swingFoceDistance, ForceMode.Impulse);
    }
}