using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineFollowAnchor : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float followDistance = 5f;
    [SerializeField] private float updateSpeed = 10f;

    private void Update()
    {
        GameObject anchor = GameObject.FindWithTag("Anchor");
        if (anchor == null || player == null) return;

        Vector3 anchorPos = anchor.transform.position;
        Vector3 playerPos = player.position;

        // Anchor -> Player の方向ベクトル
        Vector3 direction = (playerPos - anchorPos).normalized;

        // プレイヤーの背後に配置
        Vector3 targetPosition = playerPos - direction * followDistance;

        // 滑らかに追従
        transform.position = Vector3.Lerp(transform.position, targetPosition, updateSpeed * Time.deltaTime);
    }
}