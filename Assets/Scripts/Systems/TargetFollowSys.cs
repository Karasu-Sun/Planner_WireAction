using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFollowSys : MonoBehaviour
{
    public enum FollowMode { Lerp, SmoothDamp, Spring }

    [Header("Target Settings")]
    [SerializeField] private Transform target;
    [SerializeField] public Vector3 offset = new Vector3(0, 0, -3);

    [Header("Follow Settings")]
    [SerializeField] private FollowMode mode = FollowMode.SmoothDamp;
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Advanced")]
    [SerializeField] public float smoothTime = 0.3f;
    [SerializeField] private float springConstant = 16f;

    private Vector3 currentVelocity;
    private Vector3 springVelocity;

    [Header("é≤Ç≤Ç∆ÇÃâÒì]êßå‰")]
    public bool rotateX = true; // Xé≤ÇîΩâfÇ∑ÇÈÇ©
    public bool rotateY = true; // Yé≤ÇîΩâfÇ∑ÇÈÇ©
    public bool rotateZ = true; // Zé≤ÇîΩâfÇ∑ÇÈÇ©

    private void LateUpdate()
    {
        if (target == null) return;

        UpdatePosition();
        UpdateRotation();
    }

    public void SetSmoothTime(float value)
    {
        smoothTime = value;
    }

    private void UpdatePosition()
    {
        Vector3 targetPosition = target.TransformPoint(offset);

        switch (mode)
        {
            case FollowMode.Lerp:
                transform.position = Vector3.Lerp(
                    transform.position,
                    targetPosition,
                    followSpeed * Time.deltaTime
                );
                break;

            case FollowMode.SmoothDamp:
                transform.position = Vector3.SmoothDamp(
                    transform.position,
                    targetPosition,
                    ref currentVelocity,
                    smoothTime,
                    Mathf.Infinity,
                    Time.deltaTime
                );
                break;

            case FollowMode.Spring:
                Vector3 displacement = targetPosition - transform.position;
                springVelocity += displacement * (springConstant * Time.deltaTime);
                springVelocity *= Mathf.Clamp01(1 - followSpeed * Time.deltaTime);
                transform.position += springVelocity * Time.deltaTime;
                break;
        }
    }

    private void UpdateRotation()
    {
        if (target == null) return;

        // É^Å[ÉQÉbÉgï˚å¸Ç÷ÇÃâÒì]ÇåvéZ
        Quaternion targetRotation = Quaternion.LookRotation(
            target.position - transform.position,
            target.up
        );

        // é≤Ç≤Ç∆Ç…îΩâfÅEå≈íË
        Vector3 euler = targetRotation.eulerAngles;
        Vector3 current = transform.rotation.eulerAngles;

        if (!rotateX) euler.x = current.x;
        if (!rotateY) euler.y = current.y;
        if (!rotateZ) euler.z = current.z;

        // èCê≥ÇµÇΩâÒì]ÇégÇ§
        targetRotation = Quaternion.Euler(euler);

        // ÉXÉÄÅ[ÉYÇ…âÒì]
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void OnValidate()
    {
        if (!Application.isPlaying && target != null)
        {
            transform.position = target.TransformPoint(offset);
            transform.LookAt(target);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(target.TransformPoint(offset), 0.2f);
            Gizmos.DrawLine(transform.position, target.TransformPoint(offset));
        }
    }
#endif
}