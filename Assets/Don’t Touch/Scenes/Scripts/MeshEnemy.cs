using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class MeshEnemy : MonoBehaviour
{
    [SerializeField]
    NavMeshAgent agent;
    [SerializeField]
    Transform targetTransform;


    // Start is called before the first frame update
    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(targetTransform.position);
    }
}
