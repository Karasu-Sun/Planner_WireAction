using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ヒットストップを制御します
/// </summary>
public class HitStopManager : MonoBehaviour
{
    public static HitStopManager Instance { get; private set; }

    private Coroutine stopCoroutine;
    private bool isStopping = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// 指定時間ヒットストップを発生させる
    /// </summary>
    /// <param name="duration">停止時間（秒）</param>
    /// <param name="timeScale">停止中のTime.timeScale（例：0.0〜0.3）</param>
    public void Stop(float duration = 0.1f, float timeScale = 0f)
    {
        if (stopCoroutine != null)
            StopCoroutine(stopCoroutine);

        stopCoroutine = StartCoroutine(HitStopCoroutine(duration, timeScale));
    }

    private IEnumerator HitStopCoroutine(float duration, float timeScale)
    {
        if (isStopping) yield break;
        isStopping = true;

        float prevScale = Time.timeScale;
        Time.timeScale = timeScale;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = prevScale;

        isStopping = false;
        stopCoroutine = null;
    }

    /// <summary>
    /// 強制的に元の速度へ戻す
    /// </summary>
    public void ResetTimeScale()
    {
        if (stopCoroutine != null)
            StopCoroutine(stopCoroutine);

        Time.timeScale = 1f;
        isStopping = false;
        stopCoroutine = null;
    }
}