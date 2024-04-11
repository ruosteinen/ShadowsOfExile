using UnityEditor;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    public float maxDistance;
    public float affectedRadius = 1f;
    public LayerMask groundLayer;
    private Vector3 throwPosition;
    public GameObject FlammableAreaPrefab;
    
    public void Initialize(Vector3 initialThrowPosition) => throwPosition = initialThrowPosition;

    void Update()
    {
        float distanceFromThrow = Vector3.Distance(throwPosition, transform.position);
        if (distanceFromThrow > maxDistance) Destroy(gameObject);
    }
    
    
    private void OnCollisionEnter(Collision collision)
    {
        
        int layerIndex = collision.collider.gameObject.layer; 
        string layerName = LayerMask.LayerToName(layerIndex);
        
        if (collision.gameObject.CompareTag("Flammable"))
        {
            Flammable flammable = collision.gameObject.GetComponent<Flammable>();
            if (flammable != null && !flammable.isOnFire) flammable.Ignite();
        }
        
        if (layerName == "Ground")
        {
            Vector3 collisionPoint = collision.contacts[0].point;
            Instantiate(FlammableAreaPrefab, collisionPoint, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}