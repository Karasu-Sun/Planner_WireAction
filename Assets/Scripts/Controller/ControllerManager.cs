using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class ControllerDebugInfo
{
    public int index;
    public string name;
    public bool connected;
    public bool active;
}

/// <summary>
/// 接続順にリストにコントローラーを記録するクラスです
/// 切断時はそのまま席を保持し、再接続時にコントローラーは空席を検索し復帰します
/// </summary>
public class ControllerManager : MonoBehaviour
{
    [Header("表示用（デバッグ用）")]
    [SerializeField] private List<ControllerDebugInfo> debugList = new List<ControllerDebugInfo>();


    [Tooltip("アクティブ判定(秒数)※接続判定とは関係なし")]
    public float activityTimeoutSeconds = 2.0f;

    [Tooltip("入力を検出するデッドゾーン（誤差除去）")]
    public float analogDeadzone = 0.2f;

    // 接続順保持リスト（要素は接続順 index）
    private readonly List<ControllerInfo> controllers = new List<ControllerInfo>();

    public event Action<int, Gamepad> OnControllerConnected;
    public event Action<int, Gamepad> OnControllerDisconnected;

    private void OnEnable()
    {
        // 初期登録
        foreach (var gp in Gamepad.all)
            RegisterConnected(gp);

        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (!(device is Gamepad gp)) return;

        switch (change)
        {
            case InputDeviceChange.Added:
                // 新規接続
                RegisterConnected(gp);
                break;
            case InputDeviceChange.Removed:
            case InputDeviceChange.Disconnected:
                // 切断
                RegisterDisconnected(gp);
                break;
            case InputDeviceChange.Reconnected:
                // 再接続
                RegisterReconnected(gp);
                break;
            default:
                break;
        }
    }

    private void RegisterConnected(Gamepad gp)
    {
        var found = controllers.Find(ci => ci.deviceId == gp.deviceId);
        if (found != null)
        {
            found.isConnected = true;
            found.unityGamepad = gp;
            found.lastInputTime = Time.unscaledTime;
            return;
        }

        // 空席を探す
        int seatIndex = -1;
        for (int i = 0; i < controllers.Count; i++)
        {
            if (!controllers[i].isConnected)
            {
                seatIndex = i;
                break;
            }
        }

        var info = new ControllerInfo
        {
            index = seatIndex >= 0 ? seatIndex : controllers.Count, // 空席があればそこに設定
            deviceId = gp.deviceId,
            unityGamepad = gp,
            isConnected = true,
            lastInputTime = Time.unscaledTime
        };

        if (seatIndex >= 0)
        {
            // 空席に上書き
            controllers[seatIndex] = info;
        }
        else
        {
            // 空席なしなら末尾追加
            controllers.Add(info);
        }

        OnControllerConnected?.Invoke(info.index, gp);
        Debug.Log($"Controller 接続: index={info.index} id={info.deviceId} name={gp.displayName}");
    }

    private void RegisterDisconnected(Gamepad gp)
    {
        var found = controllers.Find(ci => ci.deviceId == gp.deviceId);
        if (found == null) return;

        found.isConnected = false;
        OnControllerDisconnected?.Invoke(found.index, gp);

        Debug.Log($"Controller 切断: index={found.index} id={found.deviceId} name={gp.displayName}");
    }

    private void RegisterReconnected(Gamepad gp)
    {
        RegisterConnected(gp);
    }

    private void Update()
    {
        float now = Time.unscaledTime;
        foreach (var ci in controllers)
        {
            if (!ci.isConnected) continue;
            var gp = ci.unityGamepad;
            if (gp == null) continue;

            if (IsGamepadActuated(gp))
            {
                ci.lastInputTime = now;
            }
        }

        UpdateDebugList(now);
    }

    private void UpdateDebugList(float now)
    {
        debugList.Clear();
        foreach (var ci in controllers)
        {
            debugList.Add(new ControllerDebugInfo
            {
                index = ci.index,
                name = ci.unityGamepad?.displayName ?? "Unknown",
                connected = ci.isConnected,
                active = ci.isConnected && (now - ci.lastInputTime <= activityTimeoutSeconds)
            });
        }
    }

    private bool IsGamepadActuated(Gamepad gp)
    {
        if (gp == null) return false;

        // ボタン
        if (gp.buttonSouth.isPressed) return true;
        if (gp.buttonNorth.isPressed) return true;
        if (gp.buttonWest.isPressed) return true;
        if (gp.buttonEast.isPressed) return true;
        if (gp.leftShoulder.isPressed) return true;
        if (gp.rightShoulder.isPressed) return true;
        if (gp.startButton.isPressed) return true;
        if (gp.selectButton.isPressed) return true;
        if (gp.leftStickButton.isPressed) return true;
        if (gp.rightStickButton.isPressed) return true;

        // トリガー
        if (gp.leftTrigger.ReadValue() > 0.1f) return true;
        if (gp.rightTrigger.ReadValue() > 0.1f) return true;

        // スティック
        if (Mathf.Abs(gp.leftStick.x.ReadValue()) > analogDeadzone) return true;
        if (Mathf.Abs(gp.leftStick.y.ReadValue()) > analogDeadzone) return true;
        if (Mathf.Abs(gp.rightStick.x.ReadValue()) > analogDeadzone) return true;
        if (Mathf.Abs(gp.rightStick.y.ReadValue()) > analogDeadzone) return true;

        return false;
    }

    // 公開

    //接続順のコントローラ数（これまでに登録された数）
    public int RegisteredCount => controllers.Count;

    // 指定indexのGamepadを取得（存在しない・切断時はnull）
    public Gamepad GetGamepadByIndex(int index)
    {
        var ci = controllers.Find(c => c.index == index);
        if (ci == null) return null;
        return ci.isConnected ? ci.unityGamepad : null;
    }

    // 指定 index が現在接続中かどうか
    public bool IsConnected(int index)
    {
        var ci = controllers.Find(c => c.index == index);
        return ci != null && ci.isConnected;
    }

    // 指定 index が「アクティブ（直近に入力があった）」か
    public bool IsActive(int index)
    {
        var ci = controllers.Find(c => c.index == index);
        if (ci == null || !ci.isConnected) return false;
        return (Time.unscaledTime - ci.lastInputTime) <= activityTimeoutSeconds;
    }

    // デバッグ用一覧を返す
    public IEnumerable<(int index, bool connected, bool active, string name)> GetStatusList()
    {
        foreach (var ci in controllers)
        {
            yield return (ci.index, ci.isConnected, (ci.isConnected && (Time.unscaledTime - ci.lastInputTime) <= activityTimeoutSeconds), ci.unityGamepad?.displayName ?? "Unknown");
        }
    }

    // 情報を保持する内部クラス
    [Serializable]
    private class ControllerInfo
    {
        public int index;
        public int deviceId;
        public Gamepad unityGamepad;
        public bool isConnected;
        public float lastInputTime;
    }
}