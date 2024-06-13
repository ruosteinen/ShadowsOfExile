using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            print("hit " + collision.gameObject.name + " !");
            DoDamage(collision.collider);

        }
        else if (collision.gameObject.CompareTag("Crystal"))
        {
            print("hit " + collision.gameObject.name + " !");
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
