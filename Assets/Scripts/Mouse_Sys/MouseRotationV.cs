using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseRotationV : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 5f; // ‰ñ“]‘¬“x

    private float rotationX;

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        // X²iã‰º‚Ì“®‚«j
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f); // ‰ñ“]”ÍˆÍ‚ğ§ŒÀ

        // Œ»İ‚ÌY²‰ñ“]‚ğˆÛ
        float currentY = transform.rotation.eulerAngles.y;

        // ‰ñ“]‚ğ“K—p
        transform.rotation = Quaternion.Euler(rotationX, currentY, 0f);
    }
}