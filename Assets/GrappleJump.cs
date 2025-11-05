using UnityEngine;

/// <summary>
/// グラップル終了後のジャンプ処理を担当
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class GrappleJump : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform jumpForcePoint;

    [Header("設定")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private Vector3 jumpDirection = Vector3.up;

    private bool lastLeftGrapple;
    private bool lastRightGrapple;

    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        bool isLeftGrapple = PlayerStatus.Instance.GetStatus(PlayerStatusType.IsLeftGrapple);
        bool isRightGrapple = PlayerStatus.Instance.GetStatus(PlayerStatusType.IsRightGrapple);

        if (lastLeftGrapple && lastRightGrapple)
        {
            JumpGrapple();
        }

        lastLeftGrapple = isLeftGrapple;
        lastRightGrapple = isRightGrapple;
    }

    private void JumpGrapple()
    {
        Vector3 forceDir = jumpDirection.normalized;

        if (jumpForcePoint != null)
            rb.AddForceAtPosition(forceDir * jumpForce, jumpForcePoint.position, ForceMode.Impulse);
        else
            rb.AddForce(forceDir * jumpForce, ForceMode.Impulse);
    }
}