using UnityEngine;

public class FireBall : MonoBehaviour
{
    public float maxDistance;
    public float affectedRadius = 5f;
    public LayerMask groundLayer;
    private Vector3 throwPosition;

    public void Initialize(Vector3 initialThrowPosition) => throwPosition = initialThrowPosition;

    void Update()
    {
        float distanceFromThrow = Vector3.Distance(throwPosition, transform.position);
        if (distanceFromThrow > maxDistance) Destroy(gameObject);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Flammable"))
        {
            Flammable flammable = collision.gameObject.GetComponent<Flammable>();
            if (flammable != null && !flammable.isOnFire) flammable.Ignite();
        }
        else if (groundLayer == (groundLayer | (1 << collision.gameObject.layer)))
        {
            Collider[] colliders = Physics.OverlapSphere(collision.contacts[0].point, affectedRadius);
            foreach (Collider col in colliders)
            {
                ParticleSystem ps = col.GetComponent<ParticleSystem>();
                if (ps != null) ps.Play();
            }
        }
        Destroy(gameObject);
    }
}