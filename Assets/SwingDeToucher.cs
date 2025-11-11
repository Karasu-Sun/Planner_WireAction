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

    private void FixedUpdate()
    {
        GameObject anchor = GameObject.FindWithTag("Anchor");
        if (anchor == null || player == null) return;

        Vector3 anchorPos = anchor.transform.position;
        Vector3 playerPos = player.position;

        if (playerPos.x > anchorPos.x && playerPos.y > (anchorPos.y - disTance))
        {
            grappleSysL.ForceDetach();
            grappleSysR.ForceDetach();
        }
    }
}
