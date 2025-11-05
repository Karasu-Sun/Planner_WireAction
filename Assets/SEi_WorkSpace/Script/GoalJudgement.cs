using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalJudgement : MonoBehaviour
{
    [SerializeField] private SceneManager sceneManager;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Goal");
            sceneManager.ChangeSceneResult();
            // Additional logic for when the player reaches the goal can be added here
        }
    }
}
