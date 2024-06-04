/*using UnityEngine;
using UnityEngine.AI;

public class TestRunAway : MonoBehaviour
{
    public FSMNavMeshAgent fsmNavMeshAgent;
    private NavMeshAgent agent;
    private HealthSystem healthSystem;
    public Transform[] farthestPatrolPoints;

    void Start()
    {
        agent = fsmNavMeshAgent.GetComponent<NavMeshAgent>();
        healthSystem = fsmNavMeshAgent.GetComponent<HealthSystem>();
    }

    void Update()
    {
        if (healthSystem.health <= healthSystem.maxHealth * 0.5f) // Проверка, если здоровье меньше или равно 50%
        {
            RunAway();
        }
        else
        {
            return;
        }
    }

    private void RunAway()
    {
        agent.speed *= 3f;

        if (farthestPatrolPoints != null && farthestPatrolPoints.Length > 0)
        {
            Transform nearestPoint = null;
            float shortestDistance = Mathf.Infinity;
            Vector3 currentPosition = fsmNavMeshAgent.transform.position;

            foreach (Transform point in farthestPatrolPoints)
            {
                float distance = Vector3.Distance(currentPosition, point.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestPoint = point;
                }
            }

            if (nearestPoint != null)
            {
                Debug.Log("Running away to: " + nearestPoint.position);
                agent.SetDestination(nearestPoint.position);
            }
            else
            {
                Debug.LogWarning("No valid farthest patrol points found!");
            }
        }
        else
        {
            Debug.LogWarning("Farthest Patrol Points are not set!");
        }
    }
}*/