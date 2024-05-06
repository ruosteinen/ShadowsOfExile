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
        else if(collision.gameObject.CompareTag("Flammable"))
        {
            Flammable flammable = collision.gameObject.GetComponent<Flammable>();
            if (flammable != null && !flammable.isOnFire) flammable.Ignite();
        }
        /*else if (groundLayer == (groundLayer | (1 << collision.gameObject.layer)))
        {
            Collider[] colliders = Physics.OverlapSphere(collision.contacts[0].point, affectedRadius);
            foreach (Collider col in colliders)
            {
                ParticleSystem ps = col.GetComponent<ParticleSystem>();
                if (ps != null) ps.Play();
            }
        }*/
        Destroy(gameObject);
    } 
    private void DoDamage(Collider other)
    {
        HealthSystem enemy = other.gameObject.GetComponent<HealthSystem>();
        enemy.TakeDamage(damage);
        Destroy(gameObject);
    }
}
