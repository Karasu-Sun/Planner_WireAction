using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum SceneStatusType
{
    IsCopyRight,
    IsTitle
}

public class SceneStatus : MonoBehaviour
{
    public static SceneStatus Instance { get; private set; }

    // 内部状態管理
    private readonly Dictionary<SceneStatusType, bool> statusDict = new();

    [Header("現在有効なステータス")]
    private List<string> statusList = new();

    private void Awake()
    {
        // シングルトン初期化
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeStatuses();
    }

    private void InitializeStatuses()
    {
        statusDict.Clear();
        foreach (SceneStatusType type in Enum.GetValues(typeof(SceneStatusType)))
        {
            statusDict[type] = false;
        }
        UpdateDebugList();
    }

    public void SetStatus(SceneStatusType type, bool value)
    {
        if (!statusDict.ContainsKey(type))
            statusDict.Add(type, value);
        else
            statusDict[type] = value;

        UpdateDebugList();
    }

    public bool GetStatus(SceneStatusType type)
        => statusDict.TryGetValue(type, out bool value) && value;


    public void ClearAllStatus()
    {
        foreach (var key in statusDict.Keys.ToList())
            statusDict[key] = false;

        UpdateDebugList();
    }

    private void UpdateDebugList()
    {
        statusList = statusDict
            .Where(s => s.Value)
            .Select(s => s.Key.ToString())
            .ToList();
    }
}