using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HideTest : MonoBehaviour
{
    private NavMeshAgent agent;
    public Transform[] patrolWaypoints;
    public Transform target;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void Hide()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, target.position - transform.position, out hit);
        if (hit.collider.CompareTag("Player"))
        {
            Transform point = null;
            float maxDist = 0;
            foreach (Transform w in patrolWaypoints)
            {
                float dist = Vector3.Distance(w.position, target.position);
                if (maxDist < dist)
                {
                    point = w;
                    maxDist = dist;
                }
            }
            agent.SetDestination(point.position);
        }
    }

    public bool IsAtDestination()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void Update()
    {
        if(IsAtDestination())
            Hide();
    }
}
