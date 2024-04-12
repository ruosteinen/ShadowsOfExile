using UnityEngine;

public class FireBall : MonoBehaviour
{
    public float maxDistance;
    public LayerMask groundLayer;
    private Vector3 throwPosition;
    public GameObject particleSystemPrefab;
    public GameObject particleModelPrefab;
    public float maxScale = 8.0f;

    private float currentScale = 1.0f;
    private bool isParticleSystemActivated = false;

    public void Initialize(Vector3 initialThrowPosition) => throwPosition = initialThrowPosition;

    void Update()
    {
        if (isParticleSystemActivated)
        {
            currentScale = Mathf.MoveTowards(currentScale, maxScale, (maxScale - 1.0f) / maxDistance * Time.deltaTime);
        }
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
            GameObject particleSystemObject = Instantiate(particleSystemPrefab, collisionPoint, Quaternion.identity);
            ParticleSystem.ShapeModule shapeModule = particleSystemObject.GetComponent<ParticleSystem>().shape;

            shapeModule.shapeType = ParticleSystemShapeType.Mesh;
            shapeModule.mesh = particleModelPrefab.GetComponent<MeshFilter>().sharedMesh;
            particleSystemObject.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            shapeModule.radius = currentScale;

            isParticleSystemActivated = true;
        }
        Destroy(gameObject);
    }
}
