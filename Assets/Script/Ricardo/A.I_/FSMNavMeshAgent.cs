using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GridBrushBase;

public class FSMNavMeshAgent : MonoBehaviour
{
    private NavMeshAgent agent;
    private HealthSystem healthSystem;
    public Transform[] patrolWaypoints;
    public Transform target;
    private Animator animator;

    [Header("Shooting Settings")]
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private GameObject bulletPrefab;
    public float shootTimeInterval = 2;
    private float shootTimer = 0;
    public int shootAmmountToHide = 5;
    public int shootCounter = 0;

    [Header("Enemy Settings")]
    public int health = 100;

    public int runAwayTimeInterval = 1;
    private float runAwayTimer = 0;
    private int prevWaypoint;
    public float tdcHealthHolder;

    public int meleeDamage = 20;
    public int meleeTimeInterval = 2;
    private bool doMeeleBool = false;

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
    public LayerMask m_LayerMask;
    //----

    public EnemyType type;
    public enum EnemyType
    {
        Melee,
        Ranged,
        Striker
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        healthSystem = GetComponent<HealthSystem>();
        animator= GetComponentInChildren<Animator>();

        healthSystem.maxHealth = health;
        tdcHealthHolder = health;
    }

    private void Update()
    {
        if (type != EnemyType.Striker)
        {
            if (agent.velocity.magnitude >= 0.1f)
            {
                animator.SetBool("isMoving", true);
            }
            else
            {
                animator.SetBool("isMoving", false);
            }
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

    public void GoToNextPatrolWaypoint()
    {
        int rnd = Random.Range(0, patrolWaypoints.Length);
        if (rnd != prevWaypoint)
        {
            prevWaypoint = rnd;
            agent.SetDestination(patrolWaypoints[rnd].position);
        }
    }

    public void Stop()
    {
        agent.isStopped = true;
        agent.ResetPath();
    }

    public void GoToTarget()
    {
        agent.SetDestination(target.position);
    }

    public void ResetShootCounter()
    {
        shootCounter = 0;
    }

    private void RotateToTarget()
    {
        float singleStep = rotateSpeed * Time.deltaTime;
        newDirection = (target.position - transform.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(newDirection);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, singleStep);
    }

    public void RangedAttack()
    {
        RotateToTarget();
        if (shootTimer < shootTimeInterval)
        {
            shootTimer += Time.deltaTime;
        }
        else if(shootTimer > shootTimeInterval)
        {
            animator.SetBool("Attack", true);
            StartCoroutine(StopAttack(shootTimeInterval - 0.5f));
            //shoot bullet on the right time of the animation
            StartCoroutine(ShootBullet(.6f));
            
            shootTimer = 0;
            shootCounter++;
        }
    }

    private IEnumerator ShootBullet(float time)
    {
        yield return new WaitForSeconds(time);
        Vector3 diretion = (target.position - transform.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, transform.position + transform.forward + transform.right / 2, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().AddForce(diretion * (1000 * 3));
    }

    public void MeleeAttack()
    {
        RotateToTarget();

        shootTimer += Time.deltaTime;
        if (shootTimer > meleeTimeInterval)
        {
            animator.SetBool("Attack", true);
            StartCoroutine(StopAttack(meleeTimeInterval-0.5f));
            StartCoroutine(DoMelee(.5f));

            if (doMeeleBool)
            {

                Collider[] hitColliders = Physics.OverlapBox(transform.position + transform.forward * 3, transform.localScale*1.5f, Quaternion.identity, m_LayerMask);
                int i = 0;

                while (i < hitColliders.Length)
                {
                    if (hitColliders[i].CompareTag("Player"))
                    {
                        hitColliders[i].GetComponentInParent<HealthSystem>().TakeDamage(meleeDamage);
                    }
                    i++;
                }
                if (i == hitColliders.Length)
                {
                    shootTimer = 0;
                }
            }
        }
        else if (doMeeleBool)
        {
            doMeeleBool = false;
        }
    }

    private IEnumerator DoMelee(float time)
    {
        yield return new WaitForSeconds(time);
        doMeeleBool = true;

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + transform.forward*3, transform.localScale.magnitude*1.5f);
    }

    public void StrikerAttack()
    {
        RotateToTarget();
        shootTimer += Time.deltaTime;
        if (shootTimer > shootTimeInterval * 3)
        {
            GameObject missile = Instantiate(missilePrefab, target.position + new Vector3(0, 10, 0), Quaternion.identity);
            Rigidbody rb = missile.GetComponent<Rigidbody>();
            Vector3 forceToAdd = Vector3.down * 5;
            rb.AddForce(forceToAdd, ForceMode.Force);
            shootTimer = 0;
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

    public bool DirectContactWithPlayer()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position + new Vector3(0, (transform.localScale.y/3) *2, 0), target.position - transform.position, out hit);
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

    public void SetSearchDir()
    {
        rotationDirection[0] = transform.right;
        rotationDirection[1] = -transform.right;
        rotationDirection[2] = -transform.forward;
        rotationDirection[3] = transform.forward;
    }

    public float GetHealth()
    {
        return healthSystem.health;
    }
    public float GetMaxHealth()
    {
        return healthSystem.maxHealth;
    }

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

    IEnumerator StopAttack(float time)
    {
        yield return new WaitForSeconds(time);
        animator.SetBool("Attack", false);
    }

    IEnumerator WaitFor(float time)
    {
        yield return new WaitForSeconds(time);
    }
}