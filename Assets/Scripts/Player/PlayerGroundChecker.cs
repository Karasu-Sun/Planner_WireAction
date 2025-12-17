using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundChecker : MonoBehaviour
{
    [SerializeField] private float rayDistance = 0.2f; // 足元判定の距離
    [SerializeField] private LayerMask groundMask;     // 地面レイヤーを指定

    private void Update()
    {
        bool isGrounded = Physics.Raycast(
            transform.position,         // プレイヤーの中心（必要なら足元に調整）
            Vector3.down,               // 下方向へレイ
            rayDistance,                // 判定距離
            groundMask                  // 地面だけに反応
        );

        PlayerStatus.Instance.SetStatus(PlayerStatusType.IsGround, isGrounded);
    }

    private void OnDrawGizmos()
    {
        // Sceneビューでレイを確認する用
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * rayDistance);
    }
}
