using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingRope_MLab : MonoBehaviour
{
    [Header("References")]
    public GrappleSystem grappling;

    [Header("Settings")]
    public int quality = 200;
    public float damper = 14;
    public float strength = 800;
    public float velocity = 15;
    public float waveCount = 3;
    public float waveHeight = 1;
    public AnimationCurve affectCurve;

    private Spring_MLab spring;
    private LineRenderer lr;
    private Vector3 currentGrapplePosition;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        spring = new Spring_MLab();
        spring.SetTarget(0);
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    private void DrawRope()
    {
        if (!grappling.IsGrappling)
        {
            currentGrapplePosition = grappling.injectionPoint.position;
            spring.Reset();
            if (lr.positionCount > 0)
                lr.positionCount = 0;
            return;
        }

        if (lr.positionCount == 0)
        {
            spring.SetVelocity(velocity);
            lr.positionCount = quality + 1;
        }

        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.Update(Time.deltaTime);

        Vector3 grapplePoint = grappling.GrapplePoint;
        Vector3 gunTipPosition = grappling.injectionPoint.position;
        Vector3 ropeDir = (grapplePoint - gunTipPosition).normalized;

        // ← 進行方向ベクトルから垂直なベクトルを2本生成
        Vector3 up = Quaternion.LookRotation(ropeDir) * Vector3.up;
        Vector3 right = Quaternion.LookRotation(ropeDir) * Vector3.right;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);

        for (int i = 0; i < quality + 1; i++)
        {
            float delta = i / (float)quality;

            // スパイラル回転角度を計算
            float angle = delta * waveCount * 2f * Mathf.PI; // waveCount = 回転数

            // 正弦波＋余弦波で円運動を作る
            Vector3 spiralOffset =
                (right * Mathf.Cos(angle) + up * Mathf.Sin(angle)) *
                waveHeight * spring.Value * affectCurve.Evaluate(delta);

            // 最終的な位置
            Vector3 ropePos = Vector3.Lerp(gunTipPosition, currentGrapplePosition, delta) + spiralOffset;

            lr.SetPosition(i, ropePos);
        }
    }
}
