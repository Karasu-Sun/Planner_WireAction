using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤー移動制御
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Player_MoveSystem : MonoBehaviour
{
    public enum MoveMode { TransformMove, RigidbodyMove }
    public enum DirectionMode { PlayerForward, CameraForward }
    public enum ControlMode { Controller, Keyboard }

    [Header("基本設定")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private int controllerIndex = 0;

    [Header("移動方式")]
    [SerializeField] private MoveMode moveMode = MoveMode.TransformMove;
    [SerializeField] private DirectionMode directionMode = DirectionMode.CameraForward;

    [Header("操作方式")]
    [SerializeField] private ControlMode controlMode = ControlMode.Controller;

    [Header("オプション")]
    [SerializeField] private bool rotateToMoveDirection = true;

    private Rigidbody rb;
    private Vector2 moveInput;
    private Gamepad gp;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void Update()
    {
        ReadInput();

        // 移動ステータス判定
        SetMoveStatus();

        if (moveMode == MoveMode.TransformMove)
            HandleTransformMove();
    }

    private void FixedUpdate()
    {
        if (moveMode == MoveMode.RigidbodyMove)
            HandleRigidbodyMove();
    }

    private void ReadInput()
    {
        switch (controlMode)
        {
            case ControlMode.Controller:
                if (Gamepad.all.Count > controllerIndex)
                {
                    gp = Gamepad.all[controllerIndex];
                    if (gp != null)
                        moveInput = gp.leftStick.ReadValue();
                }
                break;

            case ControlMode.Keyboard:
                float h = Input.GetAxisRaw("Horizontal");
                float v = Input.GetAxisRaw("Vertical");
                moveInput = new Vector2(h, v);
                break;
        }
    }

    private void HandleTransformMove()
    {
        float currentSpeed = GetModifiedSpeed();

        Vector3 moveDir = GetMoveDirection();
        if (moveDir.sqrMagnitude < 0.001f) return;

        if (rotateToMoveDirection)
            RotateToDirection(moveDir);

        transform.position += moveDir * currentSpeed * Time.deltaTime;
    }

    private void HandleRigidbodyMove()
    {
        float currentSpeed = GetModifiedSpeed();

        Vector3 moveDir = GetMoveDirection();
        if (moveDir.sqrMagnitude < 0.001f) return;

        if (rotateToMoveDirection)
            RotateToDirection(moveDir);

        Vector3 targetPos = rb.position + moveDir * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(targetPos);
    }

    private Vector3 GetMoveDirection()
    {
        if (moveInput.sqrMagnitude < 0.01f)
            return Vector3.zero;

        Vector3 forward, right;

        if (directionMode == DirectionMode.CameraForward && Camera.main != null)
        {
            forward = Camera.main.transform.forward;
            right = Camera.main.transform.right;
        }
        else
        {
            forward = transform.forward;
            right = transform.right;
        }

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        return (forward * moveInput.y + right * moveInput.x).normalized;
    }

    private void RotateToDirection(Vector3 dir)
    {
        if (dir.sqrMagnitude < 0.001f) return;
        Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
    }

    // -----------------------------
    // 追加部分: 移動ステータス判定と速度変更
    // -----------------------------
    private void SetMoveStatus()
    {
        bool isMoving = moveInput.sqrMagnitude > 0.01f;
        PlayerStatus.Instance.SetStatus(PlayerStatusType.IsMove, isMoving);
        PlayerStatus.Instance.SetStatus(PlayerStatusType.IsStandby, !isMoving);
    }

    [SerializeField, Tooltip("低加速度係数")]
    private float downSpeedDistance = 0.5f;

    [SerializeField, Tooltip("上昇速度係数")]
    private float upSpeedDistance = 1.5f;

    /// <summary>
    /// 移動速度にステータス効果を反映
    /// </summary>
    private float GetModifiedSpeed()
    {
        float modifiedSpeed = moveSpeed;

        // ステータスによる補正
        if (PlayerStatus.Instance.GetStatus(PlayerStatusType.IsCrouch)
            || PlayerStatus.Instance.GetStatus(PlayerStatusType.IsJump))
            modifiedSpeed *= downSpeedDistance; // 速度低下
        if (PlayerStatus.Instance.GetStatus(PlayerStatusType.IsSprint))
            modifiedSpeed *= upSpeedDistance; // 速度上昇

        return modifiedSpeed;
    }
}