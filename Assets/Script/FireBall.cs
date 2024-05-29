using UnityEngine;

public class FireBall : MonoBehaviour
{
    public float maxDistance;
    private Vector3 throwPosition;
    public int damage;

    private PlayStats playStats;

    public void Initialize(Vector3 initialThrowPosition) => throwPosition = initialThrowPosition;

    void Update()
    {
        float distanceFromThrow = Vector3.Distance(throwPosition, transform.position);
        if (distanceFromThrow > maxDistance) Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            DoDamage(collision.collider);
        }
        else if (collision.gameObject.CompareTag("Flammable"))
        {
            Flammable flammable = collision.gameObject.GetComponent<Flammable>();
            if (flammable != null && !flammable.isOnFire) flammable.Ignite();
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
