using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    /// <summary>
    /// Continuous movement provider that automatically determines
    /// the frame of reference (head or hand) used for movement based on the player's settings.
    /// </summary>
    [AddComponentMenu("XR/Locomotion/Dynamic Move Provider (Action Based)")]
    public class DynamicMoveProvider : ActionBasedContinuousMoveProvider
    {
        /// <summary>
        /// Defines which transform the XR Origin's movement direction is relative to.
        /// </summary>
        public enum MovementDirection
        {
            /// <summary>
            /// Use the forward direction of the head (camera).
            /// </summary>
            HeadRelative,

            /// <summary>
            /// Use the forward direction of the controller (hand).
            /// </summary>
            HandRelative,
        }

        [Space, Header("Movement Direction Settings")]

        [SerializeField, Tooltip("Transform used when moving relative to the head (camera). If null, automatically finds XR Origin's camera.")]
        private Transform m_HeadTransform;

        [SerializeField, Tooltip("Transform used when moving relative to the left hand.")]
        private Transform m_LeftControllerTransform;

        [SerializeField, Tooltip("Transform used when moving relative to the right hand.")]
        private Transform m_RightControllerTransform;

        [SerializeField, Tooltip("Whether to use head-relative or hand-relative movement for the left hand.")]
        private MovementDirection m_LeftHandMovementDirection = MovementDirection.HeadRelative;

        [SerializeField, Tooltip("Whether to use head-relative or hand-relative movement for the right hand.")]
        private MovementDirection m_RightHandMovementDirection = MovementDirection.HeadRelative;

        private Transform m_CombinedTransform;
        private Pose m_LeftMovementPose = Pose.identity;
        private Pose m_RightMovementPose = Pose.identity;

        /// <summary>Head transform reference.</summary>
        public Transform headTransform
        {
            get => m_HeadTransform;
            set => m_HeadTransform = value;
        }

        /// <summary>Left controller transform reference.</summary>
        public Transform leftControllerTransform
        {
            get => m_LeftControllerTransform;
            set => m_LeftControllerTransform = value;
        }

        /// <summary>Right controller transform reference.</summary>
        public Transform rightControllerTransform
        {
            get => m_RightControllerTransform;
            set => m_RightControllerTransform = value;
        }

        /// <summary>Movement direction mode for the left hand.</summary>
        public MovementDirection leftHandMovementDirection
        {
            get => m_LeftHandMovementDirection;
            set => m_LeftHandMovementDirection = value;
        }

        /// <summary>Movement direction mode for the right hand.</summary>
        public MovementDirection rightHandMovementDirection
        {
            get => m_RightHandMovementDirection;
            set => m_RightHandMovementDirection = value;
        }

        protected override void Awake()
        {
            base.Awake();

            // Create a combined transform to average both controllers' movement direction
            m_CombinedTransform = new GameObject("[Dynamic Move Provider] Combined Forward Source").transform;
            m_CombinedTransform.SetParent(transform, false);
            m_CombinedTransform.localPosition = Vector3.zero;
            m_CombinedTransform.localRotation = Quaternion.identity;

            forwardSource = m_CombinedTransform;
        }

        /// <inheritdoc />
        protected override Vector3 ComputeDesiredMove(Vector2 input)
        {
            if (input == Vector2.zero)
                return Vector3.zero;

            EnsureHeadTransform();

            // Determine the movement poses for left and right
            m_LeftMovementPose = GetPoseForDirection(m_LeftHandMovementDirection, m_HeadTransform, m_LeftControllerTransform);
            m_RightMovementPose = GetPoseForDirection(m_RightHandMovementDirection, m_HeadTransform, m_RightControllerTransform);

            // Read values safely from InputActionProperty
            Vector2 leftValue = Vector2.zero;
            if (leftHandMoveAction != null && leftHandMoveAction.action != null)
                leftValue = leftHandMoveAction.action.ReadValue<Vector2>();

            Vector2 rightValue = Vector2.zero;
            if (rightHandMoveAction != null && rightHandMoveAction.action != null)
                rightValue = rightHandMoveAction.action.ReadValue<Vector2>();

            // Blend based on input strength
            float totalSqrMagnitude = leftValue.sqrMagnitude + rightValue.sqrMagnitude;
            float leftBlend = (totalSqrMagnitude > Mathf.Epsilon)
                ? leftValue.sqrMagnitude / totalSqrMagnitude
                : 0.5f;

            var combinedPosition = Vector3.Lerp(m_RightMovementPose.position, m_LeftMovementPose.position, leftBlend);
            var combinedRotation = Quaternion.Slerp(m_RightMovementPose.rotation, m_LeftMovementPose.rotation, leftBlend);
            m_CombinedTransform.SetPositionAndRotation(combinedPosition, combinedRotation);

            return base.ComputeDesiredMove(input);
        }

        private void EnsureHeadTransform()
        {
            if (m_HeadTransform != null)
                return;

            var xrOrigin = system.xrOrigin;
            if (xrOrigin != null && xrOrigin.Camera != null)
                m_HeadTransform = xrOrigin.Camera.transform;
        }

        private static Pose GetPoseForDirection(MovementDirection direction, Transform head, Transform controller)
        {
            return direction switch
            {
                MovementDirection.HeadRelative when head != null => new Pose(head.position, head.rotation),
                MovementDirection.HandRelative when controller != null => new Pose(controller.position, controller.rotation),
                _ => Pose.identity,
            };
        }
    }
}