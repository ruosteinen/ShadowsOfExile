using UnityEngine;

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
        
        if (collision.gameObject.CompareTag("Enemy")) DoDamage(collision.collider);
        
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
        Destroy(gameObject);
    }
    
    private void DoDamage(Collider other)
    {
        HealthSystem enemy = other.gameObject.GetComponent<HealthSystem>();

        enemy.TakeDamage(damage);

        Destroy(gameObject);
    }
}