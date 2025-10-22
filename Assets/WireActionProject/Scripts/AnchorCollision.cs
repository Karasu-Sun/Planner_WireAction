using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorCollision : MonoBehaviour
{
    public XRGrapplingHook grapplingHook;

    private void Awake()
    {
        // ÉVÅ[Éìè„ÇÃXRGrapplingHookÇé©ìÆÇ≈éÊìæ
        if (grapplingHook == null)
        {
            grapplingHook = FindObjectOfType<XRGrapplingHook>();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") && grapplingHook != null)
        {
            grapplingHook.FixAnchor(transform, collision.contacts[0].point);
        }
    }
}