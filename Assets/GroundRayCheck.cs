using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundRayCheck : MonoBehaviour
{
    [SerializeField] private float rayLength = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Rigidbody rb;

    private void FixedUpdate()
    {
        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, rayLength, groundLayer))
        {
            // ‰ºŒü‚«‚É—Ž‚¿‚Ä‚¢‚éŽž‚¾‚¯Ž~‚ß‚é
            if (rb.velocity.y <= 0f)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            }
        }
    }
}