using UnityEngine;
using Unity.AI;
public class WaterBall : MonoBehaviour
{
    public float maxDistance;
    public int damage;
    private Vector3 throwPosition;

    public void Initialize(Vector3 initialThrowPosition) => throwPosition = initialThrowPosition;
    void Update()
    {
        float distanceFromThrow = Vector3.Distance(throwPosition, transform.position);
        if (distanceFromThrow > maxDistance) Destroy(gameObject);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("WaterBall collided with: " + collision.gameObject.name);
    
        if (collision.gameObject.CompareTag("Enemy")) 
        {
            /* Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            if (enemyRigidbody != null)
            {
                Vector3 direction = collision.contacts[0].point - transform.position;
                enemyRigidbody.AddForce(direction.normalized * knockbackForce, ForceMode.Impulse);
            }
        
            UnityEngine.AI.NavMeshAgent enemyAgent = collision.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (enemyAgent != null) enemyAgent.velocity *= slowFactor;*/
            
            DoDamage(collision.collider);
        }
        else if (collision.gameObject.CompareTag("Flammable"))
        {
            Flammable flammable = collision.gameObject.GetComponent<Flammable>();
            if (flammable != null)
            {
                if (flammable.isOnFire)
                {
                    Debug.Log("Extinguishing fire on: " + collision.gameObject.name);
                    flammable.Extinguish();
                }
                else Debug.Log("Hit flammable object, but it's not on fire: " + collision.gameObject.name);
            }
            else Debug.Log("Flammable obj not found on: " + collision.gameObject.name);
        }
        else if (collision.gameObject.CompareTag("Crystal"))
        {
            DoDamage(collision.collider);
        }

        Destroy(gameObject);
    }

    
    private void DoDamage(Collider other)
    {
        HealthSystem enemy = other.gameObject.GetComponent<HealthSystem>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Debug.Log("Damaged Enemy: " + other.gameObject.name);
        }
        else
        {
            DarkCrystalManager crystal = other.gameObject.GetComponent<DarkCrystalManager>();
            if (crystal != null)
            {
                crystal.TakeDamage(damage);
                Debug.Log("Damaged Crystal: " + other.gameObject.name);
            }
            else
            {
                Debug.Log("No valid component found on: " + other.gameObject.name);
            }
        }
    }
}
