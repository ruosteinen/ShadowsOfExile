using UnityEngine;

public class FireBall : MonoBehaviour
{
    public float maxDistance;
    private Vector3 throwPosition;

    private void Update()
    {
        if (Vector3.Distance(transform.position, throwPosition) > maxDistance) Destroy(gameObject);
    }
    
    public void Initialize(Vector3 initialThrowPosition) => throwPosition = initialThrowPosition;


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Flammable"))
        {
            Flammable flammable = collision.gameObject.GetComponent<Flammable>();
            if (flammable != null && !flammable.isOnFire) flammable.Ignite();
        }
        Destroy(gameObject);
    }
}
