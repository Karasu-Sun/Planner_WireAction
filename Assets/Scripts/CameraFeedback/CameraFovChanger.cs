using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カメラのFOVを制御します
/// </summary>
public class CameraFovChanger : MonoBehaviour
{
    public static CameraFovChanger Instance { get; private set; }

    [Header("対象カメラ（未指定時は自動でCamera.main）")]
    [SerializeField] private Camera targetCamera;

    [Header("変化に使用するEasingカーブ")]
    [Tooltip("未指定の場合はMathf.SmoothStep")]
    [SerializeField] private AnimationCurve easingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Coroutine fovCoroutine;
    private float defaultFov;

    // 記録用
    private float currentTargetFov;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera != null)
            defaultFov = targetCamera.fieldOfView;
    }

    /// <summary>
    /// FOVを滑らかに変化、指定時間保持して戻す
    /// </summary>
    /// <param name="targetFov">目標FOV</param>
    /// <param name="changeSpeed">変化速度（1秒あたりの進行度）</param>
    /// <param name="holdTime">保持時間（秒）</param>
    public void ChangeFov(float targetFov, float changeSpeed = 5f, float holdTime = 0.5f)
    {
        if (targetCamera == null) return;

        // 前回コルーチンが動作中なら停止
        if (fovCoroutine != null)
            StopCoroutine(fovCoroutine);

        fovCoroutine = StartCoroutine(ChangeFovCoroutine(targetFov, changeSpeed, holdTime));
    }

    /// <summary>
    /// 即座にFOVをデフォルト値へ戻す
    /// </summary>
    public void ResetFov()
    {
        if (targetCamera == null) return;

        if (fovCoroutine != null)
        {
            StopCoroutine(fovCoroutine);
            fovCoroutine = null;
        }

        targetCamera.fieldOfView = defaultFov;
    }

    /// <summary>
    /// カメラFOVをデフォルト値として記録
    /// </summary>
    public void SetDefaultFov(float newFov)
    {
        defaultFov = newFov;
        if (targetCamera != null)
            targetCamera.fieldOfView = newFov;
    }

    /// <summary>
    /// 現在のFOVを取得
    /// </summary>
    public float GetCurrentFov()
    {
        return targetCamera ? targetCamera.fieldOfView : defaultFov;
    }

    private IEnumerator ChangeFovCoroutine(float targetFov, float changeSpeed, float holdTime)
    {
        float startFov = targetCamera.fieldOfView;
        currentTargetFov = targetFov;
        float t = 0f;

        // 目標値までEasingで変化
        while (t < 1f)
        {
            t += Time.deltaTime * changeSpeed;
            float easedT = easingCurve != null ? easingCurve.Evaluate(t) : Mathf.SmoothStep(0f, 1f, t);
            targetCamera.fieldOfView = Mathf.Lerp(startFov, targetFov, easedT);
            yield return null;
        }

        targetCamera.fieldOfView = targetFov;

        // 指定秒数保持
        yield return new WaitForSeconds(holdTime);

        // 元に戻す
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * changeSpeed;
            float easedT = easingCurve != null ? easingCurve.Evaluate(t) : Mathf.SmoothStep(0f, 1f, t);
            targetCamera.fieldOfView = Mathf.Lerp(targetFov, defaultFov, easedT);
            yield return null;
        }

        targetCamera.fieldOfView = defaultFov;
        fovCoroutine = null;
    }
}