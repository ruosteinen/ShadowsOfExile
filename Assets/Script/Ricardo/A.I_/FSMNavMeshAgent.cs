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
<<<<<<< HEAD
    [SerializeField] private GameObject bulletPrefab;
=======
    [SerializeField] private GameObject arrowPrefab;
>>>>>>> parent of 592e66d (A.I. working)
    public float shootTimeInterval = 2;
    private float shootTimer = 0;
    public int shootCounter = 0;

    [Header("Enemy Settings")]
    public int health = 100;
<<<<<<< HEAD
=======
    public int healthToRecover = 20;

    public float recoverTimeInterval = 2f;
    private float recoverTimer;

    public int runAwayTimeInterval = 1;
    private float runAwayTimer = 0;
    private int prevWaypoint;
>>>>>>> parent of 592e66d (A.I. working)
    public float tdcHealthHolder;

    public int meleeDamage = 20;
    public int contactDamage = 5; 
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
<<<<<<< HEAD
        
=======
        if (IsAtDestination())
        {
            GoToNextPatrolWaypoint();
        }
>>>>>>> parent of 592e66d (A.I. working)
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
<<<<<<< HEAD
            StopAttack(shootTimeInterval - 0.5f);
            ShootBullet(.6f);
=======
            StartCoroutine(StopAttack(shootTimeInterval - 0.5f));
            //shoot bullet on the right time of the animation
            StartCoroutine(ShootBullet(.6f));
>>>>>>> parent of 592e66d (A.I. working)

            shootTimer = 0;
            shootCounter++;
        }
    }

    private void ShootBullet(float time)
    {
<<<<<<< HEAD
        Invoke("InstantiateBullet", time);
=======
        yield return new WaitForSeconds(time);
        Vector3 diretion = (target.position - transform.position).normalized;
        GameObject arrow = Instantiate(arrowPrefab, transform.position + transform.forward + transform.right / 2, Quaternion.identity);
        arrow.GetComponent<Rigidbody>().AddForce(diretion * (1000 * 3));
>>>>>>> parent of 592e66d (A.I. working)
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


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Access the player's health system and deal damage
            collision.gameObject.GetComponent<PlayerHealthSystem>().TakeDamage(contactDamage);
            //Debug.Log("work plz");
        }
    }



    public void MeleeAttack()
    {
        RotateToTarget();

        shootTimer += Time.deltaTime;
        if (shootTimer > meleeTimeInterval)
        {
<<<<<<< HEAD
            StopAttack(meleeTimeInterval - 0.5f);
            DoMelee(.5f);

            if (doMeleeBool)
=======
            StartCoroutine(StopAttack(meleeTimeInterval - 0.5f));
            StartCoroutine(DoMelee(.5f));

            if (doMeeleBool)
>>>>>>> parent of 592e66d (A.I. working)
            {
                Collider[] hitColliders = Physics.OverlapBox(transform.position + transform.forward * 3, transform.localScale * 1.5f);
                int i = 0;

<<<<<<< HEAD
=======
                Collider[] hitColliders = Physics.OverlapBox(transform.position + transform.forward * 3, transform.localScale * 1.5f, Quaternion.identity, m_LayerMask);
                int i = 0;

>>>>>>> parent of 592e66d (A.I. working)
                while (i < hitColliders.Length)
                {
                    if (hitColliders[i].CompareTag("Player"))
                    {
<<<<<<< HEAD
                        // Call the TakeDamage method of the player's health system
                        hitColliders[i].GetComponentInParent<PlayerHealthSystem>().TakeDamage(meleeDamage);
                        Debug.Log("dam");
=======
                        hitColliders[i].GetComponentInParent<HealthSystem>().TakeDamage(meleeDamage);
>>>>>>> parent of 592e66d (A.I. working)
                    }
                    i++;
                }
                if (i == hitColliders.Length)
                {
                    shootTimer = 0;
                }
            }
        }
<<<<<<< HEAD
        else if (doMeleeBool)
        {
            doMeleeBool = false;
=======
        else if (doMeeleBool)
        {
            doMeeleBool = false;
>>>>>>> parent of 592e66d (A.I. working)
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

<<<<<<< HEAD
=======
    public void Recover()
    {
        if (agent.velocity.magnitude < 0.1f)
        {
            recoverTimer += Time.deltaTime;
            if (healthSystem.health < healthSystem.maxHealth && recoverTimer > recoverTimeInterval)
            {
                healthSystem.RecoverHealth(healthToRecover);
                UpdateHealthHolder();
                recoverTimer = 0;
            }
        }
    }

    public void Search()
    {
        float singleStep = rotateSpeed * Time.deltaTime;

        if (Quaternion.Angle(transform.rotation, Quaternion.LookRotation(rotationDirection[rotationNum])) <= 2)
        {
            if (rotationNum < rotationDirection.Length - 1)
            {
                rotationNum++;
            }
            else
            {
                rotationComplete = true;
            }
        }
        else
        {
            if (Quaternion.Angle(transform.rotation, Quaternion.LookRotation(rotationDirection[rotationNum])) >= 2 && rotationNum < rotationDirection.Length)
            {
                newDirection = Vector3.RotateTowards(transform.forward, rotationDirection[rotationNum], singleStep, 0.0f);

                transform.rotation = Quaternion.LookRotation(newDirection);
            }
        }
    }

    public void RunAway()
    {
        if (!agent.hasPath)
        {
            runAwayTimer += Time.deltaTime;
            if (runAwayTimer > runAwayTimeInterval)
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
                runAwayTimer = 0;
            }
        }
    }

>>>>>>> parent of 592e66d (A.I. working)
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

<<<<<<< HEAD
=======
    public void UpdateHealthHolder()
    {
        tdcHealthHolder = GetHealth();
    }

    public bool CanHide()
    {
        if (shootCounter >= shootAmmountToHide)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

>>>>>>> parent of 592e66d (A.I. working)
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
<<<<<<< HEAD
        // No need for delay, directly reset the timer
        shootTimer = 0;
=======
        yield return new WaitForSeconds(time);
        Debug.Log("Stopattack");
>>>>>>> parent of 592e66d (A.I. working)
    }
}
