using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SwingDeToucher : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GrappleSystem grappleSysL;
    [SerializeField] private GrappleSystem grappleSysR;
    [SerializeField] private float disTance = 2.0f;

    private Vector3? anchorPos = null;

    private void OnEnable()
    {
        GrappleSystem.OnAnchorCreated += HandleAnchorCreated;
    }

    private void HandleAnchorCreated(Vector3 pos)
    {
        anchorPos = pos;
    }

    private void Update()
    {
        if (anchorPos == null) return;

        Vector3 a = anchorPos.Value;
        Vector3 p = player.position;

        bool isRight = p.z > a.z;
        bool isAbove = p.y > (a.y - disTance);

        if (isRight && isAbove)
        {
            grappleSysL.StopGrapple();
            grappleSysR.StopGrapple();
        }
    }
}