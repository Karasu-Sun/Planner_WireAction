using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCatcher : MonoBehaviour
{
    [SerializeField] private MonoBehaviour targetScript;

    [SerializeField] private Transform startPoint;

    [SerializeField] private GrappleSystem grappleSysL;
    [SerializeField] private GrappleSystem grappleSysR;

    private void Awake()
    {
        if (startPoint != null)
        {
            transform.position = startPoint.position;
            transform.rotation = startPoint.rotation;
        }

        if (targetScript != null)
        {
            targetScript.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Anchor"))
        {
            targetScript.enabled = true;

            grappleSysL.StopGrapple();
            grappleSysR.StopGrapple();

            StartCoroutine(SetGameStartAfterDelay(4f));
        }
    }

    private IEnumerator SetGameStartAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneStatus.Instance.SetStatus(SceneStatusType.IsItemCatch, true);
        yield return new WaitForSeconds(delay);
        SceneStatus.Instance.SetStatus(SceneStatusType.IsItemCatch, false);
        SceneStatus.Instance.SetStatus(SceneStatusType.IsGameStart, true);
        Debug.Log("Game Start!");
    }
}