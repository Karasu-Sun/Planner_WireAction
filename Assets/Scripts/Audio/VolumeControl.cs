
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

//======================================
// データ構造
//======================================
[System.Serializable]
public class VolumeEntry
{
    [Header("識別情報")]
    public string label; // 表示用ラベル
    public string mixerParameter; // AudioMixer パラメータ名
    public string prefsKey; // 保存キー

    [Header("UI参照")]
    public Text displayText; // 音量表示テキスト
    public Slider slider; // 音量スライダー
}

public class VolumeControl : MonoBehaviour
{
    //======================================
    // インスペクタ設定
    //======================================
    [Header("音量設定リスト")]
    [SerializeField] private List<VolumeEntry> volumeEntries = new List<VolumeEntry>();

    [Header("UI フェード制御")]
    [SerializeField] private CanvasGroup volumePanel;
    [SerializeField] private float fadeSpeed = 5f;

    [Header("オーディオ")]
    [SerializeField] private AudioMixer audioMixer;

    //======================================
    // 内部変数
    //======================================
    private float targetAlpha = 0f;

    // 外部操作用フラグ
    private bool isOptionActive = false;
    public bool IsOptionActive
    {
        get => isOptionActive;
        set => SetOptionActive(value);
    }

    //======================================
    // 外部公開メソッド
    //======================================
    public void SetOptionActive(bool active)
    {
        isOptionActive = active;
    }

    public void ToggleOptionPanel()
    {
        SetOptionActive(!isOptionActive);
    }

    //======================================
    // ライフサイクル
    //======================================
    private void Start()
    {
        foreach (var entry in volumeEntries)
            InitializeEntry(entry);
    }

    private void Update()
    {
        if (volumePanel == null) return;

        // オプション表示状態に応じてフェード
        targetAlpha = isOptionActive ? 1f : 0f;

        volumePanel.alpha = Mathf.MoveTowards(
            volumePanel.alpha,
            targetAlpha,
            Time.unscaledDeltaTime * fadeSpeed
        );

        bool isVisible = volumePanel.alpha > 0.01f;
        volumePanel.interactable = isVisible;
        volumePanel.blocksRaycasts = isVisible;
    }

    //======================================
    // 初期化処理
    //======================================
    private void InitializeEntry(VolumeEntry entry)
    {
        if (entry.slider == null) return;

        float savedValue = PlayerPrefs.GetFloat(entry.prefsKey, 0.5f);
        entry.slider.value = savedValue;

        ApplyVolume(entry, savedValue);

        entry.slider.onValueChanged.AddListener(value =>
        {
            ApplyVolume(entry, value);
            PlayerPrefs.SetFloat(entry.prefsKey, value);
        });

        DisableKeyboardNavigation(entry.slider);
    }

    //======================================
    // 音量適用・表示更新
    //======================================
    private void ApplyVolume(VolumeEntry entry, float value)
    {
        UpdateMixer(entry.mixerParameter, value);
        UpdateDisplay(entry, value);
    }

    private void UpdateMixer(string parameter, float value)
    {
        float dB = value > 0f ? 20f * Mathf.Log10(value) : -80f;

        if (!audioMixer.SetFloat(parameter, dB))
            Debug.LogError($"AudioMixer パラメータ '{parameter}' が見つかりません");
    }

    private void UpdateDisplay(VolumeEntry entry, float value)
    {
        if (entry.displayText != null)
            entry.displayText.text = $"{entry.label}: {Mathf.RoundToInt(value * 100)}%";
    }

    //======================================
    // スライダー操作制限
    //======================================
    private void DisableKeyboardNavigation(Slider slider)
    {
        if (slider == null) return;
        slider.navigation = new Navigation { mode = Navigation.Mode.None };
    }
}