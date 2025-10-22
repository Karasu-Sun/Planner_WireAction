using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRGrapplingHook : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Rigidbody playerRigidbody;
    public XRProjectileShooter projectileShooter;

    [Header("Settings")]
    public string wallTag = "Wall";

    private Transform currentAnchor;
    private bool anchorFixed = false;
    private float maxDistance;

    private void Update()
    {
        if (anchorFixed && currentAnchor != null)
        {
            ApplyRopeConstraint();
        }
    }

    // Anchorが壁に当たった時に呼ぶ
    public void FixAnchor(Transform anchor, Vector3 hitPoint)
    {
        currentAnchor = anchor;
        anchorFixed = true;
        currentAnchor.position = hitPoint;

        Rigidbody rb = currentAnchor.GetComponent<Rigidbody>();
        if (rb != null) Destroy(rb); // Anchorを固定

        maxDistance = Vector3.Distance(player.position, currentAnchor.position);
    }

    public void ReleaseGrappling()
    {
        if (currentAnchor != null)
        {
            Destroy(currentAnchor.gameObject); // Anchorを削除
            currentAnchor = null;
            anchorFixed = false;
            maxDistance = 0f;
        }
    }

    private void ApplyRopeConstraint()
    {
        Vector3 toAnchor = player.position - currentAnchor.position;
        float distance = toAnchor.magnitude;

        if (distance > maxDistance)
        {
            // プレイヤー位置を補正
            Vector3 correctedPosition = currentAnchor.position + toAnchor.normalized * maxDistance;
            player.position = correctedPosition;

            // ロープ方向の速度を打ち消す
            Vector3 velocityTowardsAnchor = Vector3.Project(playerRigidbody.velocity, toAnchor.normalized);
            playerRigidbody.velocity -= velocityTowardsAnchor;
        }
    }
}