using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤー移動制御
/// カメラ基準・自身基準、Transform/Rigidbody移動を切替可能。
/// </summary>
/// 
public enum PlayerStatusType
{
    IsStandby,
    IsSprint,
    IsCrouch,
    IsGround,
    IsJump,
    IsMove,
    IsGrappling,
    IsRightGrapple,
    IsLeftGrapple,
}

public class PlayerStatus : MonoBehaviour
{
    public static PlayerStatus Instance { get; private set; }

    private Dictionary<PlayerStatusType, bool> statusDict = new Dictionary<PlayerStatusType, bool>();

    [SerializeField, Tooltip("現在のステータス")]
    private List<string> statusList = new List<string>();

    public event Action<PlayerStatusType, bool> OnStatusChanged;

        // ステータスの初期化
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        foreach (PlayerStatusType type in Enum.GetValues(typeof(PlayerStatusType)))
        {
            statusDict[type] = false;
        }

        UpdateDebugStatus();
    }

    // ステータスの取得
    public bool GetStatus(PlayerStatusType statusType)
    {
        return statusDict.TryGetValue(statusType, out bool value) && value;
    }

        // ステータスの変更
    public void SetStatus(PlayerStatusType statusType, bool value)
    {
        if (statusDict.ContainsKey(statusType) && statusDict[statusType] != value)
        {
            statusDict[statusType] = value;
            OnStatusChanged?.Invoke(statusType, value);

            UpdateDebugStatus();
        }
    }

    // 表示ステータスの更新
    private void UpdateDebugStatus()
    {
        statusList.Clear();

        foreach (var status in statusDict)
        {
            if (status.Value)
            {
                statusList.Add(status.Key.ToString());
            }
        }
    }
}