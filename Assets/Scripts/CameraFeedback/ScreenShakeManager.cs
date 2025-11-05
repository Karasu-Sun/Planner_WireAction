using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スクリーンシェイクを制御します
/// </summary>
public class ScreenShakeManager : MonoBehaviour
{
    public static ScreenShakeManager Instance { get; private set; }

    [Header("基本設定")]
    [SerializeField] private Transform targetCamera; // シェイク対象
    [SerializeField] private AnimationCurve shakeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    [SerializeField] private float defaultDuration = 0.2f;
    [SerializeField] private float defaultIntensity = 0.3f;
    [SerializeField] private float randomOffset = 0.2f; // 揺れに加えるノイズ

    private Vector3 originalPos;
    private Coroutine shakeRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (targetCamera == null)
        {
            targetCamera = Camera.main?.transform;
        }
        if (targetCamera != null)
        {
            originalPos = targetCamera.localPosition;
        }
    }

    /// <summary>
    /// 指定方向にスクリーンシェイクを発生
    /// Vector3 hitDir = (enemy.position - player.position).normalized;
    /// ScreenShakeManager.Instance.Shake(hitDir, intensity, duration);
    /// </summary>
    public void Shake(Vector3 hitDirection, float intensity = -1f, float duration = -1f, bool syncWithHitstop = true)
    {
        if (targetCamera == null) return;

        if (intensity <= 0) intensity = defaultIntensity;
        if (duration <= 0) duration = defaultDuration;

        // 既存の揺れを停止
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(ShakeRoutine(hitDirection, intensity, duration, syncWithHitstop));
    }

    private IEnumerator ShakeRoutine(Vector3 direction, float intensity, float duration, bool syncWithHitstop)
    {
        // カメラ基準化
        direction.Normalize();
        originalPos = targetCamera.localPosition;

        float time = 0f;

        // ヒットストップ中はシェイクを待機させる
        if (syncWithHitstop)
        {
            yield return new WaitUntil(() => Time.timeScale > 0.05f);
        }

        while (time < duration)
        {
            float normalized = time / duration;
            float curveValue = shakeCurve.Evaluate(normalized);
            float currentIntensity = intensity * curveValue;

            // 基本方向 + ノイズ
            Vector3 randomNoise = new Vector3(
                Random.Range(-randomOffset, randomOffset),
                Random.Range(-randomOffset, randomOffset),
                0f);

            Vector3 offset = (direction * currentIntensity) + randomNoise;

            targetCamera.localPosition = originalPos + offset;

            time += Time.unscaledDeltaTime; // ヒットストップ中も動作可能
            yield return null;
        }

        targetCamera.localPosition = originalPos;
        shakeRoutine = null;
    }

    /// <summary>
    /// 強制停止して元の位置に戻す
    /// </summary>
    public void ResetShake()
    {
        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
            shakeRoutine = null;
        }
        if (targetCamera != null)
        {
            targetCamera.localPosition = originalPos;
        }
    }
}