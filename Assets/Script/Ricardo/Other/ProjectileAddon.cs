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
        if (other.gameObject.CompareTag("Enemy"))
        {
            HealthSystem enemyHealth = other.gameObject.GetComponent<HealthSystem>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            PlayStats playerHealth = other.gameObject.GetComponent<PlayStats>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
