using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class FSMNavMeshAgent : MonoBehaviour
{
    private NavMeshAgent agent;
    private HealthSystem healthSystem;
    public Transform[] patrolWaypoints;
    public Transform target;

    [Header("Shooting Settings")]
    [SerializeField] private GameObject bulletPrefab;
    public float shootTimeInterval = 2;
    private float shootTimer = 0;
    public int shootCounter = 0;

    [Header("Enemy Settings")]
    public int health = 100;
    public float tdcHealthHolder;

    public int meleeDamage = 20;
    public int meleeTimeInterval = 2;
    private bool doMeleeBool = false;

    // Search Variables to be changed
    public float rotateSpeed = 2f;
    public bool rotationComplete;
    private Vector3[] rotationDirection =
        {
            Vector3.right,
            Vector3.left,
            Vector3.back,
            Vector3.forward
        };
    private Vector3 newDirection;
    public int rotationNum = 0;

    //----
    public EnemyType type;
    public enum EnemyType
    {
        Melee,
        Ranged
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        healthSystem = GetComponent<HealthSystem>();

        healthSystem.maxHealth = health;
    }

    private void Update()
    {
        
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

    public void GoToNextPatrolWaypoint()
    {
        int rnd = Random.Range(0, patrolWaypoints.Length);
        agent.SetDestination(patrolWaypoints[rnd].position);
    }

    public void Stop()
    {
        agent.isStopped = true;
        agent.ResetPath();
    }

    public void GoToTarget(float stoppingDistance)
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget > stoppingDistance)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Vector3 destination = target.position - direction * stoppingDistance;
            agent.SetDestination(destination);
        }
    }


    public void ResetShootCounter()
    {
        shootCounter = 0;
    }

    private void RotateToTarget()
{
    Vector3 direction = target.position - transform.position;
    direction.y = 0; // Set the y-component to 0 to prevent up and down rotation
    Quaternion rotation = Quaternion.LookRotation(direction);
    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotateSpeed);
}


    public void RangedAttack()
    {
        RotateToTarget();
        if (shootTimer < shootTimeInterval)
        {
            shootTimer += Time.deltaTime;
        }
        else if (shootTimer > shootTimeInterval)
        {
            StopAttack(shootTimeInterval - 0.5f);
            ShootBullet(.6f);

            shootTimer = 0;
            shootCounter++;
        }
    }

    private void ShootBullet(float time)
    {
        Invoke("InstantiateBullet", time);
    }

    private void InstantiateBullet()
    {
        // Define the distance in front of the enemy to spawn the bullet
        float spawnDistance = 2f; // Adjust as needed

        // Calculate the position in front of the enemy
        Vector3 spawnPosition = transform.position + transform.forward * spawnDistance;

        // Instantiate the bullet at the calculated position
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

        // Add force to the bullet in the direction of the target
        Vector3 direction = (target.position - spawnPosition).normalized;
        bullet.GetComponent<Rigidbody>().AddForce(direction * (1000 * 3));
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Access the player's health system and deal damage
            other.GetComponent<PlayerHealthSystem>().TakeDamage(meleeDamage);

            // Disable the collider temporarily
            GetComponent<Collider>().enabled = false;

            // Optionally, set a delay and re-enable the collider
            StartCoroutine(EnableColliderAfterDelay());
        }
    }

    private IEnumerator EnableColliderAfterDelay()
    {
        yield return new WaitForSeconds(1f); // Adjust the delay as needed
        GetComponent<Collider>().enabled = true;
    }


    public void MeleeAttack()
    {
        RotateToTarget();

        shootTimer += Time.deltaTime;
        if (shootTimer > meleeTimeInterval)
        {
            StopAttack(meleeTimeInterval - 0.5f);
            DoMelee(.5f);

            if (doMeleeBool)
            {
                Collider[] hitColliders = Physics.OverlapBox(transform.position + transform.forward * 3, transform.localScale * 1.5f);
                int i = 0;

                while (i < hitColliders.Length)
                {
                    if (hitColliders[i].CompareTag("Player"))
                    {
                        // Call the TakeDamage method of the player's health system
                        hitColliders[i].GetComponentInParent<PlayerHealthSystem>().TakeDamage(meleeDamage);
                    }
                    i++;
                }
                if (i == hitColliders.Length)
                {
                    shootTimer = 0;
                }
            }
        }
        else if (doMeleeBool)
        {
            doMeleeBool = false;
        }
    }

    private void DoMelee(float time)
    {
        Invoke("SetDoMeleeTrue", time);
    }

    private void SetDoMeleeTrue()
    {
        doMeleeBool = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + transform.forward * 3, transform.localScale.magnitude * 1.5f);
    }

    public bool DirectContactWithPlayer()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position + new Vector3(0, (transform.localScale.y / 3) * 2, 0), target.position - transform.position, out hit);
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else { return false; }

    }

    public void Search()
    {
        float singleStep = rotateSpeed * Time.deltaTime; // Calculate rotation speed

        if (Quaternion.Angle(transform.rotation, Quaternion.LookRotation(rotationDirection[rotationNum])) <= 90)
        {
            // If the angle between the current rotation and the target rotation is less than or equal to 90 degrees
            if (rotationNum < rotationDirection.Length - 1)
            {
                // If there are more rotation directions left in the array
                rotationNum++; // Move to the next rotation direction
            }
            else
            {
                rotationComplete = true; // Mark rotation as complete
            }
        }
        else
        {
            // If the angle between the current rotation and the target rotation is greater than 90 degrees
            if (Quaternion.Angle(transform.rotation, Quaternion.LookRotation(rotationDirection[rotationNum])) >= 90 && rotationNum < rotationDirection.Length)
            {
                // If there are more rotation directions left in the array
                newDirection = Vector3.RotateTowards(transform.forward, rotationDirection[rotationNum], singleStep, 0.0f); // Calculate new direction to rotate towards
                transform.rotation = Quaternion.LookRotation(newDirection); // Rotate towards the new direction
            }
        }
    }



    public void SetSearchDir()
    {
        rotationDirection[0] = transform.right;
        rotationDirection[1] = -transform.right;
        rotationDirection[2] = -transform.forward;
        rotationDirection[3] = transform.forward;
    }

    public void UpdateHealthHolder()
    {
    }

    public float GetHealth()
    {
        return healthSystem.health;
    }
    public float GetMaxHealth()
    {
        return healthSystem.maxHealth;
    }

    public bool CheckEnemyType(EnemyType enemyType)
    {
        if (enemyType == type)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void StopAttack(float time)
    {
        // No need for delay, directly reset the timer
        shootTimer = 0;
    }
}
