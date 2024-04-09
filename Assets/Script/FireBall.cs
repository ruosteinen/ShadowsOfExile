using UnityEngine;

public class FireBall : MonoBehaviour
{
    public float maxDistance;
    private Vector3 throwPosition;

    public void Initialize(Vector3 initialThrowPosition)
    {
        throwPosition = initialThrowPosition;
    }

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
            //Destroy(gameObject);
        }
        Destroy(gameObject);
    }
}