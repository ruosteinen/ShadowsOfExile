using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class WaterEffect : MonoBehaviour
{
    public float slowFactor = 0.1f;
    public float slowDuration = 5f;
    private NavMeshAgent agent;
    private float initialSpeed;
    private bool isSlowed;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        initialSpeed = agent.speed; 
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("WaterBall"))
        {
            Rigidbody waterBallRb = other.gameObject.GetComponent<Rigidbody>();
            if (waterBallRb != null)
            {
                Vector3 repulsionDirection = waterBallRb.velocity.normalized;
                float repulsionStrength = 1.5f;
                Repulse(repulsionDirection, repulsionStrength);
            }
            if (!isSlowed) StartCoroutine(SlowDown());
        }
    }

    private void Repulse(Vector3 direction, float strength)
    {
        Vector3 repulsionVector = direction * strength;
        agent.Move(repulsionVector);
    }



    private IEnumerator SlowDown()
    {
        isSlowed = true;
        float reducedSpeed = agent.speed * slowFactor;

        float startTime = Time.time;
        float elapsedTime = 0f;
        
        while (elapsedTime < slowDuration)
        {
            elapsedTime = Time.time - startTime;
            agent.speed = Mathf.Lerp(initialSpeed, reducedSpeed, elapsedTime / slowDuration);
            yield return null;
        }
        
        yield return new WaitForSeconds(slowDuration);
        
        startTime = Time.time;
        elapsedTime = 0f;
        
        while (elapsedTime < slowDuration)
        {
            elapsedTime = Time.time - startTime;
            agent.speed = Mathf.Lerp(reducedSpeed, initialSpeed, elapsedTime / slowDuration);
            yield return null;
        }

        agent.speed = initialSpeed;
        isSlowed = false;
    }
}