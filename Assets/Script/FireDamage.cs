using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireDamage : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem;
    private List<ParticleCollisionEvent> collisionEvents = new ();
    public int damage;

    private void Start()
    {
        if (!particleSystem) particleSystem = GetComponent<ParticleSystem>();
    }

    private void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = particleSystem.GetCollisionEvents(other, collisionEvents);

        for (int i = 0; i < numCollisionEvents; i++)
        {
            Collider collider = collisionEvents[i].colliderComponent as Collider;
            if (collider && collider.CompareTag("Enemy")) DoDamage(collider);
            if (collider && collider.CompareTag("Player")) TakeDamageToPlayer(collider);
        }
    }

    private void TakeDamageToPlayer(Collider collider)
    {
        PlayerHealthSystem playerHealth = collider.GetComponent<PlayerHealthSystem>();
        if (playerHealth != null) playerHealth.TakeDamage(damage);
    }

    
    private void DoDamage(Collider collider)
    {
        HealthSystem enemyHealth = collider.GetComponent<HealthSystem>();
        if (enemyHealth != null) enemyHealth.TakeDamage(damage);
    }
}