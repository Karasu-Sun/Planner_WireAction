using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplePullTightener : MonoBehaviour
{
    [Header("対象のグラップルシステム")]
    [SerializeField] private GrappleSystem targetGrappleSystem;

    [Header("短縮設定")]
    [SerializeField, Tooltip("短縮後の長さ（メートル）")]
    private float targetDistance = 3f;

    [SerializeField, Tooltip("短縮速度（m/s）")]
    private float shortenSpeed = 10f;

    private bool wasRightGrapple = false;
    private bool isShortening = false;

    private void Update()
    {
        bool isRightGrapple = PlayerStatus.Instance.GetStatus(PlayerStatusType.IsRightGrapple);

        // 接続された瞬間に短縮を開始
        if (!wasRightGrapple && isRightGrapple)
        {
            StartShorten();
        }

        wasRightGrapple = isRightGrapple;

        // 毎フレーム短縮処理
        if (isShortening)
        {
            ShortenJoint();
        }
    }

    private void StartShorten()
    {
        if (targetGrappleSystem == null) return;

        isShortening = true;
    }

    private void ShortenJoint()
    {
        if (targetGrappleSystem == null) return;

        var joint = targetGrappleSystem.GetComponent<SpringJoint>();
        if (joint == null)
        {
            isShortening = false;
            return;
        }

        // 現在距離を徐々に短くする
        joint.maxDistance = Mathf.MoveTowards(joint.maxDistance, targetDistance, shortenSpeed * Time.deltaTime);
        joint.minDistance = Mathf.MoveTowards(joint.minDistance, targetDistance, shortenSpeed * Time.deltaTime);

        // 目標距離に到達したら終了
        if (Mathf.Approximately(joint.maxDistance, targetDistance))
        {
            isShortening = false;
        }
    }
}