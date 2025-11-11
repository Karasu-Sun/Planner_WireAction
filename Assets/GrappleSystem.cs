using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public enum HandType { Left, Right }

public class GrappleSystem : MonoBehaviour
{
    [Header("参照")]
    public Transform player;
    public Rigidbody playerRigidbody;
    public Transform injectionPoint;

    [Header("設定")]
    public HandType handType = HandType.Right;
    public KeyCode inputKey = KeyCode.Mouse0;
    public LayerMask grappleLayer;
    public float maxDistance = 20f;
    public bool debugEnabled = true;
    public float debugSphereSize = 0.2f;

    [Header("スイング設定")]
    public float swingForce = 10f;

    private float currentMaxDistance;
    private SpringJoint springJoint;
    private Vector3 grapplePoint;
    private bool grappling;

    private GameObject debugAnchor;

    public bool IsGrappling => grappling;
    public Vector3 GrapplePoint => grapplePoint;

    [SerializeField] private InputActionReference shootAction;

    private void OnEnable()
    {
        shootAction.action.performed += OnShootPerformed;
        shootAction.action.Enable();
    }

    private void OnDisable()
    {
        shootAction.action.performed -= OnShootPerformed;
        shootAction.action.Disable();
    }

    private void OnShootPerformed(InputAction.CallbackContext context)
    {
        if (!grappling)
            TryStartGrapple();
        else
            StopGrapple();
    }

    private void Update()
    {
        HandleInput();
        if (debugEnabled) UpdateDebugRay();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(inputKey))
        {
            if (!grappling)
                TryStartGrapple();
            else
                StopGrapple();
        }
    }

    private void FixedUpdate()
    {
        if (grappling && springJoint != null)
        {
            ApplySwingMovement();
            ApplyRopeConstraint();
        }
    }

    /// <summary>
    /// グラップル開始
    /// </summary>
    private void TryStartGrapple()
    {
        Ray ray = new Ray(injectionPoint.position, injectionPoint.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, grappleLayer))
        {
            grapplePoint = hit.point;
            if (debugEnabled) DebugDrawPoint(grapplePoint, GetHandColor());

            grappling = true;
            StartSwingGrapple(grapplePoint);
            SetPlayerStatus(true);
        }
        else
        {
            grapplePoint = ray.GetPoint(maxDistance);
            if (debugEnabled) DebugDrawPoint(grapplePoint, Color.gray);

            grappling = true;
            Invoke(nameof(StopGrapple), 0.3f);
        }
    }

    /// <summary>
    /// スイング開始
    /// </summary>
    private void StartSwingGrapple(Vector3 target)
    {
        if (springJoint != null)
            Destroy(springJoint);

        springJoint = gameObject.AddComponent<SpringJoint>();
        springJoint.autoConfigureConnectedAnchor = false;
        springJoint.connectedAnchor = target;

        float distance = Vector3.Distance(transform.position, target);
        currentMaxDistance = distance;
        springJoint.maxDistance = currentMaxDistance;
        springJoint.minDistance = currentMaxDistance;
        springJoint.spring = 0f;
        springJoint.damper = 0f;
        springJoint.massScale = 1f;
    }

    /// <summary>
    /// グラップル停止
    /// </summary>
    public void StopGrapple()
    {
        grappling = false;

        if (springJoint != null)
            Destroy(springJoint);

        if (debugAnchor != null)
            Destroy(debugAnchor);

        playerRigidbody.velocity = Vector3.zero;
        SetPlayerStatus(false);
    }

    /// <summary>
    /// スイング物理制御
    /// </summary>
    private void ApplySwingMovement()
    {
        if (springJoint == null) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 swingDir = transform.right * h + transform.forward * v;

        //playerRigidbody.AddForce(swingDir * swingForce, ForceMode.Acceleration);
    }

    /// <summary>
    /// ロープ距離制約
    /// </summary>
    private void ApplyRopeConstraint()
    {
        Vector3 toAnchor = player.position - grapplePoint;
        float distance = toAnchor.magnitude;

        if (distance > currentMaxDistance)
        {
            Vector3 corrected = grapplePoint + toAnchor.normalized * currentMaxDistance;
            player.position = corrected;

            Vector3 velocityTowardsAnchor = Vector3.Project(playerRigidbody.velocity, toAnchor.normalized);
            playerRigidbody.velocity -= velocityTowardsAnchor;
        }
    }

    /// <summary>
    /// デバッグ用球生成
    /// </summary>
    private void DebugDrawPoint(Vector3 pos, Color color)
    {
        if (debugAnchor != null)
            Destroy(debugAnchor);

        debugAnchor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        debugAnchor.tag = "Anchor";
        Destroy(debugAnchor.GetComponent<Collider>());
        debugAnchor.transform.position = pos;
        debugAnchor.transform.localScale = Vector3.one * debugSphereSize;
        debugAnchor.GetComponent<Renderer>().material.color = color;
    }

    private void UpdateDebugRay()
    {
        Debug.DrawRay(injectionPoint.position, injectionPoint.forward * maxDistance, GetHandColor());
    }

    private Color GetHandColor() => handType == HandType.Right ? Color.red : Color.blue;

    /// <summary>
    /// プレイヤーステータス更新
    /// </summary>
    private void SetPlayerStatus(bool isGrappling)
    {
        if (handType == HandType.Right)
            PlayerStatus.Instance.SetStatus(PlayerStatusType.IsRightGrapple, isGrappling);
        else
            PlayerStatus.Instance.SetStatus(PlayerStatusType.IsLeftGrapple, isGrappling);
    }

    /// <summary>
    /// グラップルを強制的に切断する
    /// </summary>
    public void ForceDetach()
    {
        if (springJoint != null)
        {
            Destroy(springJoint);
            springJoint = null;
        }

        grappling = false;

        SetPlayerStatus(false);

        if (debugAnchor != null)
        {
            Destroy(debugAnchor);
            debugAnchor = null;
        }

        // 移動中の慣性を残す
        playerRigidbody.velocity = Vector3.zero;
    }
}