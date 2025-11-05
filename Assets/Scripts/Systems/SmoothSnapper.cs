using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 特定のタグを持つ対象にスナップする
/// このスクリプトは「スナップされる側」にアタッチする
/// </summary>
public class SmoothSnapper : MonoBehaviour
{
    [Header("設定")]
    [Tooltip("スナップ対象とするタグ名")]
    [SerializeField] private string targetTag = "Player";

    [Tooltip("スナップのスピード（秒間の補間速度）")]
    [SerializeField] private float defaultSpeed = 10f;

    [Tooltip("スナップ完了距離")]
    [SerializeField] private float stopDistance = 0.01f;

    private Coroutine snapRoutine;

    /// <summary>
    /// sourceをtargetに向けて滑らかにスナップ
    /// </summary>
    public void SnapToTarget(Transform source, Transform target, float speed = -1f)
    {
        if (source == null || target == null) return;
        if (speed <= 0) speed = defaultSpeed;

        if (snapRoutine != null)
            StopCoroutine(snapRoutine);

        snapRoutine = StartCoroutine(SnapRoutine(source, target, speed));
    }

    private IEnumerator SnapRoutine(Transform source, Transform target, float speed)
    {
        while (target != null && Vector3.Distance(source.position, target.position) > stopDistance)
        {
            source.position = Vector3.Lerp(source.position, target.position, Time.deltaTime * speed);
            yield return null;
        }

        snapRoutine = null;
    }

    /// <summary>
    /// 進行中のスナップを強制終了
    /// </summary>
    public void StopSnap()
    {
        if (snapRoutine != null)
        {
            StopCoroutine(snapRoutine);
            snapRoutine = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            SnapToTarget(transform, other.transform);
        }
    }
}