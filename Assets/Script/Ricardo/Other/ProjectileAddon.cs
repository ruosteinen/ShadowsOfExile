using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAddon : MonoBehaviour
{
    public int damage;

    private void Start()
    {
        Invoke("DestroyBullet", 1f);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.GetComponent<HealthSystem>() != null || other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.name.Contains(gameObject.tag) && other.gameObject.CompareTag("Enemy"))
            {
                DoDamage(other);
            }
            else if (other.gameObject.CompareTag("Player"))
            {
                HealthSystem player = other.gameObject.GetComponentInParent<HealthSystem>();

                player.TakeDamage(damage);

                Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }

    private void DoDamage(Collider other)
    {
        HealthSystem enemy = other.gameObject.GetComponent<HealthSystem>();

        enemy.TakeDamage(damage);

        Destroy(gameObject);
    }
}
