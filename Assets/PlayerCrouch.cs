using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーのしゃがみ（Crouch）管理クラス
/// </summary>
public class PlayerCrouch : MonoBehaviour
{
    private bool isCrouching = false;

    private void Update()
    {
        HandleCrouchInput();
        SetCrouchStatus();
    }

    private void HandleCrouchInput()
    {
        isCrouching = Input.GetKey(KeyCode.LeftControl);
    }

    /// <summary>
    /// 現在しゃがんでいるか
    /// </summary>
    public bool IsCrouching => isCrouching;

    private void SetCrouchStatus()
    {
        if (IsCrouching)
            PlayerStatus.Instance.SetStatus(PlayerStatusType.IsCrouch, true);
        else
            PlayerStatus.Instance.SetStatus(PlayerStatusType.IsCrouch, false);
    }
}