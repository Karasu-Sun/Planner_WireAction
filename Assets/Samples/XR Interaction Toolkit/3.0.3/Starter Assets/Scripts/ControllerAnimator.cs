using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    public class ControllerAnimator : MonoBehaviour
    {
        [Header("Thumbstick")]
        [SerializeField] Transform m_ThumbstickTransform;
        [SerializeField] Vector2 m_StickRotationRange = new Vector2(30f, 30f);
        [SerializeField] InputActionProperty m_ThumbstickAction;

        [Header("Trigger")]
        [SerializeField] Transform m_TriggerTransform;
        [SerializeField] Vector2 m_TriggerXAxisRotationRange = new Vector2(0f, -15f);
        [SerializeField] InputActionProperty m_TriggerAction;

        [Header("Grip")]
        [SerializeField] Transform m_GripTransform;
        [SerializeField] Vector2 m_GripRightRange = new Vector2(-0.0125f, -0.011f);
        [SerializeField] InputActionProperty m_GripAction;

        void Update()
        {
            if (m_ThumbstickTransform && m_ThumbstickAction.action != null)
            {
                var stickVal = m_ThumbstickAction.action.ReadValue<Vector2>();
                m_ThumbstickTransform.localRotation =
                    Quaternion.Euler(-stickVal.y * m_StickRotationRange.x, 0f, -stickVal.x * m_StickRotationRange.y);
            }

            if (m_TriggerTransform && m_TriggerAction.action != null)
            {
                var triggerVal = m_TriggerAction.action.ReadValue<float>();
                m_TriggerTransform.localRotation =
                    Quaternion.Euler(Mathf.Lerp(m_TriggerXAxisRotationRange.x, m_TriggerXAxisRotationRange.y, triggerVal), 0f, 0f);
            }

            if (m_GripTransform && m_GripAction.action != null)
            {
                var gripVal = m_GripAction.action.ReadValue<float>();
                var pos = m_GripTransform.localPosition;
                m_GripTransform.localPosition =
                    new Vector3(Mathf.Lerp(m_GripRightRange.x, m_GripRightRange.y, gripVal), pos.y, pos.z);
            }
        }
    }
}