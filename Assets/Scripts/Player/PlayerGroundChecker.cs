using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundChecker : MonoBehaviour
{
    [SerializeField] private float rayDistance = 0.2f; // 足元判定の距離
    [SerializeField] private LayerMask groundMask;     // 地面レイヤーを指定
    [SerializeField] private Rigidbody rb;

    private void Update()
    {
        bool isGrounded = Physics.Raycast(
            transform.position,         // プレイヤーの中心（必要なら足元に調整）
            Vector3.down,               // 下方向へレイ
            rayDistance,                // 判定距離
            groundMask                  // 地面だけに反応
        );

        PlayerStatus.Instance.SetStatus(PlayerStatusType.IsGround, isGrounded);

        // 地面に接している間は落下させない
        if (isGrounded)
        {
            if (rb.velocity.y < 0f)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            }

            rb.useGravity = false;
        }
        else
        {
            rb.useGravity = true;
        }
    }

    private void OnDrawGizmos()
    {
        // Sceneビューでレイを確認する用
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * rayDistance);
    }
}
