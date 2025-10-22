using Oculus.Interaction.Locomotion;
using Oculus.Interaction;
using UnityEngine;

public class TeleportController : MonoBehaviour
{
    [SerializeField]
    private ActiveStateGate teleportActiveStateGate;

    [SerializeField]
    private TeleportInteractor teleportInteractor;

    private OVRPlayerController ovrPlayerController;
    private UnityEngine.CharacterController characterController; // <- 完全修飾名

    private float halfHeight;
    private bool previousState = false;

    void Start()
    {
        SetComponent();
        SetWhenLocomotionPerformed(true);
    }

    void OnDisable()
    {
        SetWhenLocomotionPerformed(false);
    }

    void LateUpdate()
    {
        CheckTeleportActiveState();
    }

    private void SetComponent()
    {
        this.ovrPlayerController = this.GetComponent<OVRPlayerController>();
        this.characterController = this.GetComponent<UnityEngine.CharacterController>(); // <- 完全修飾名

        this.halfHeight = this.characterController.height / 2;
    }

    private void SetWhenLocomotionPerformed(bool state)
    {
        if (state)
        {
            this.teleportInteractor.WhenLocomotionPerformed += OnTeleportOcurred;
        }
        else
        {
            this.teleportInteractor.WhenLocomotionPerformed -= OnTeleportOcurred;
        }
    }

    private void CheckTeleportActiveState()
    {
        if (this.teleportActiveStateGate.Active)
        {
            if (!this.previousState)
            {
                this.previousState = true;
                this.ovrPlayerController.enabled = false;
                this.characterController.enabled = false;
            }
        }
        else
        {
            if (this.previousState)
            {
                this.previousState = false;
                this.ovrPlayerController.enabled = true;
                this.characterController.enabled = true;
            }
        }
    }

    private void OnTeleportOcurred(LocomotionEvent locEvent)
    {
        this.transform.position = new Vector3(locEvent.Pose.position.x, locEvent.Pose.position.y + halfHeight, locEvent.Pose.position.z);

        this.ovrPlayerController.enabled = true;
        this.characterController.enabled = true;
    }
}