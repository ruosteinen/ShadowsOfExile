using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroundFire : MonoBehaviour
{
    public GameObject particleSystemPrefab;
    public GameObject particleModelPrefab;
    public float maxScale = 4f;
    public float scaleSpeed = 0.1f;
    public float particleSystemDuration = 8f;

    private List<ParticleSystem> activeParticleSystems = new ();

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("FireBall"))
        {
            Vector3 collisionPoint = collision.contacts[0].point;
            GameObject particleSystemObject = Instantiate(particleSystemPrefab, collisionPoint, Quaternion.identity);
            ParticleSystem particleSystemComponent = particleSystemObject.GetComponent<ParticleSystem>();

            ParticleSystem.ShapeModule shapeModule = particleSystemComponent.shape;
            shapeModule.shapeType = ParticleSystemShapeType.Mesh;
            shapeModule.mesh = particleModelPrefab.GetComponent<MeshFilter>().sharedMesh;

            particleSystemObject.transform.localScale = new Vector3(1, 1, 1);
            activeParticleSystems.Add(particleSystemComponent);
            StartCoroutine(ScaleOverTimeAndDestroy(particleSystemObject, maxScale, scaleSpeed, particleSystemDuration));
        }

        if (collision.gameObject.CompareTag("WaterBall"))
        {
            ParticleSystem psToRemove = null; // Initialize variable to store particle system to remove from list

            foreach (ParticleSystem ps in activeParticleSystems)
            {
                // Calculate the distance between ps and collision point
                float distance = Vector3.Distance(ps.transform.position, collision.contacts[0].point);
                if (distance < ps.transform.localScale.x * 2f || distance < ps.transform.localScale.y * 2f || distance < ps.transform.localScale.z * 2f)
                {
                    ps.Stop();
                    psToRemove = ps; // Store reference to the particle system to remove from list
                }
            }
            if (psToRemove != null) activeParticleSystems.Remove(psToRemove); 
        }
    }

    private IEnumerator ScaleOverTimeAndDestroy(GameObject particleSystemObject, float maxScale, float scaleSpeed, float duration)
    {
        ParticleSystem ps = particleSystemObject.GetComponent<ParticleSystem>();
        float currentScale = 1.0f;
        float timer = 0;

        while (currentScale < maxScale)
        {
            currentScale = Mathf.Lerp(1, maxScale, timer / scaleSpeed);
            particleSystemObject.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            timer += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(duration);

        activeParticleSystems.Remove(ps);
        Destroy(particleSystemObject);
    }
}
