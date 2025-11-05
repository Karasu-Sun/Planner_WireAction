using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーのスプリント管理クラス
/// </summary>
public class PlayerSprint : MonoBehaviour
{
    private bool isSprinting = false;

    private void Update()
    {
        HandleSprintInput();
        SetSprintStatus();
    }

    private void HandleSprintInput()
    {
        isSprinting = Input.GetKey(KeyCode.LeftShift);
    }

    /// <summary>
    /// 現在スプリント中か
    /// </summary>
    public bool IsSprinting => isSprinting;

    private void SetSprintStatus()
    {
        if (IsSprinting)
            PlayerStatus.Instance.SetStatus(PlayerStatusType.IsSprint, true);
        else 
            PlayerStatus.Instance.SetStatus(PlayerStatusType.IsSprint, false);
    }
}