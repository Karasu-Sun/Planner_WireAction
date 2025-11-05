using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// éwíËÇµÇΩStrongGrappleActivatorÇÃMidpointÇ…í«è]Ç∑ÇÈ
/// </summary>
public class FollowMidpoint : MonoBehaviour
{
    [SerializeField] private StrongGrappleActivator activator;
    [SerializeField] private Vector3 offset = Vector3.zero;
    [SerializeField] private float smoothSpeed = 5f;

    private void Update()
    {
        if (activator == null) return;

        Vector3 targetPosition = activator.Midpoint + offset;

        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}