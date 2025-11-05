using UnityEngine;

/// <summary>
/// 片方のグラップルが接続されたとき、もう片方を切断する
/// </summary>
public class GrappleLinkBreaker : MonoBehaviour
{
    [Header("対象グラップルシステム")]
    [Tooltip("このスクリプトが監視する側")]
    [SerializeField] private GrappleSystem selfGrapple;

    [Tooltip("接続時に切断命令を送る相手側グラップル")]
    [SerializeField] private GrappleSystem otherGrapple;

    [Header("状態検知")]
    [Tooltip("監視対象ステータスタイプ")]
    [SerializeField] private PlayerStatusType grappleStatusType;

    [Header("遅延設定")]
    [Tooltip("切断までのインターバル（秒）")]
    [SerializeField] private float detachDelay = 0.2f;

    private bool wasConnected = false;
    private bool isWaitingToDetach = false;
    private float detachTimer = 0f;

    private void Update()
    {
        bool isConnected = PlayerStatus.Instance.GetStatus(grappleStatusType);

        // 接続された瞬間を検知
        if (isConnected && !wasConnected)
        {
            OnConnected();
        }

        // 遅延中ならタイマー更新
        if (isWaitingToDetach)
        {
            detachTimer += Time.deltaTime;
            if (detachTimer >= detachDelay)
            {
                ExecuteDetach();
            }
        }

        wasConnected = isConnected;
    }

    /// <summary>
    /// 接続時に他方のグラップルを切断予約
    /// </summary>
    private void OnConnected()
    {
        if (otherGrapple == null)
        {
            Debug.LogWarning($"{name}: 切断対象のグラップルが指定されていません。");
            return;
        }

        isWaitingToDetach = true;
        detachTimer = 0f;
    }

    /// <summary>
    /// 切断実行処理
    /// </summary>
    private void ExecuteDetach()
    {
        isWaitingToDetach = false;

        if (otherGrapple != null)
        {
            otherGrapple.ForceDetach();
            //Debug.Log($"{name}: {otherGrapple.name} に切断信号を送信しました。（遅延 {detachDelay:F2}s）");
        }
    }
}