using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpForce = 10f;
    public float jumpCooldown = 0.25f;

    [Header("References")]
    public Rigidbody rb;
    public Transform playerTransform;
    public LayerMask whatIsGround;
    public float playerHeight = 2f;

    private bool readyToJump = true;
    private bool grounded;

    private void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
        if (playerTransform == null)
            playerTransform = transform;
    }

    private void Update()
    {
        GroundCheck();
        HandleJumpInput();
        SetGroundStatus();
        SetJumpStatus();
    }

    private void GroundCheck()
    {
        grounded = Physics.Raycast(playerTransform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
    }

    private void HandleJumpInput()
    {
        if (Input.GetKey(KeyCode.Space) && readyToJump && grounded)
        {
            Jump();
            readyToJump = false;
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    public void Jump()
    {
        // y方向の速度をリセットしてからジャンプ
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    public bool IsJumping => !readyToJump;
    public bool IsGrounded => grounded;

    private void SetGroundStatus()
    {
        if (IsGrounded)
            PlayerStatus.Instance.SetStatus(PlayerStatusType.IsGround, true);
        else
            PlayerStatus.Instance.SetStatus(PlayerStatusType.IsGround, false);
    }
    private void SetJumpStatus()
    {
        if (!IsGrounded)
            PlayerStatus.Instance.SetStatus(PlayerStatusType.IsJump, true);
        else
            PlayerStatus.Instance.SetStatus(PlayerStatusType.IsJump, false);
    }
}