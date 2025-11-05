using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 2つのオブジェクトが一定距離以内にあるとき、
/// 中間地点に強力なグラップルガンを有効化する。
/// </summary>
public class StrongGrappleActivator : MonoBehaviour
{
    [Header("監視対象オブジェクト")]
    [SerializeField] private Transform targetA;
    [SerializeField] private Transform targetB;

    [Header("発動設定")]
    [Tooltip("発動する距離（これ以下になると有効化）")]
    [SerializeField] private float activationDistance = 5f;

    [Tooltip("有効化する強力なグラップルガン（プレハブ or 非アクティブオブジェクト）")]
    [SerializeField] private GameObject strongGrappleGunPrefab;

    [Tooltip("既に存在する場合、再生成しない")]
    [SerializeField] private bool allowMultiple = false;

    private GameObject activeGrappleGun;
    private bool isActive = false;

    private void Update()
    {
        if (targetA == null || targetB == null) return;

        float distance = Vector3.Distance(targetA.position, targetB.position);

        if (distance <= activationDistance)
        {
            if (!isActive)
            {
                ActivateStrongGrapple();
            }
        }
        else
        {
            if (isActive)
            {
                DeactivateStrongGrapple();
            }
        }
    }

    /// <summary>
    /// 強力なグラップルガンを中間地点に有効化
    /// </summary>
    private void ActivateStrongGrapple()
    {
        isActive = true;

        Vector3 midpoint = (targetA.position + targetB.position) / 2f;
        Quaternion rotation = Quaternion.LookRotation(targetB.position - targetA.position);

        if (strongGrappleGunPrefab == null)
        {
            Debug.LogWarning($"{name}: 強力なグラップルガンが指定されていません。");
            return;
        }

        if (!allowMultiple && activeGrappleGun != null)
        {
            // 既に存在していれば再生成しない
            return;
        }

        activeGrappleGun = Instantiate(strongGrappleGunPrefab, midpoint, rotation);
        Debug.Log($"{name}: 強力なグラップルガンを有効化しました。位置: {midpoint}");
    }

    /// <summary>
    /// 強力なグラップルガンを無効化または削除
    /// </summary>
    private void DeactivateStrongGrapple()
    {
        isActive = false;

        if (activeGrappleGun != null)
        {
            Destroy(activeGrappleGun);
            activeGrappleGun = null;
            Debug.Log($"{name}: 強力なグラップルガンを無効化しました。");
        }
    }

    /// <summary>
    /// 外部から参照できるリアルタイムの中間地点
    /// </summary>
    public Vector3 Midpoint
    {
        get
        {
            if (targetA != null && targetB != null)
                return (targetA.position + targetB.position) / 2f;
            return transform.position;
        }
    }

    /// <summary>
    /// デバッグ用：シーンビューに可視化
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (targetA == null || targetB == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(targetA.position, targetB.position);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere((targetA.position + targetB.position) / 2f, 0.2f);
    }
}