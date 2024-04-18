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
        }
    }

    private void DoDamage(Collider collider)
    {
        HealthSystem enemyHealth = collider.GetComponent<HealthSystem>();
        if (enemyHealth != null) enemyHealth.TakeDamage(damage);
    }
}