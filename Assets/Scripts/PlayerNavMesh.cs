using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerNavMesh : MonoBehaviour
{
    [SerializeField]
    Transform target;
    [SerializeField]
    NavMeshAgent navMeshAgent;

    void Update()
    {
        navMeshAgent.destination = target.position;
    }
}
