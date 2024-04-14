using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroundFire : MonoBehaviour
{
    public GameObject fireParticleSystemPrefab;
    public GameObject smokeParticleSystemPrefab;
    public GameObject particleModelPrefab;
    public float maxScale = 4f;
    public float scaleSpeed = 0.1f;
    public float particleSystemDuration = 8f;
    public float smokeDelay = 1.0f; // Delay before smoke starts

    private List<ParticleSystem> activeParticleSystems = new List<ParticleSystem>();

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("FireBall"))
        {
            Vector3 collisionPoint = collision.contacts[0].point;
            StartCoroutine(StartFireAndSmokeSystems(collisionPoint));
        }
        else if (collision.gameObject.CompareTag("WaterBall"))
        {
            Vector3 collisionPoint = collision.contacts[0].point;
            StopParticleSystemAtPoint(collisionPoint);
        }
    }

    private IEnumerator StartFireAndSmokeSystems(Vector3 collisionPoint)
    {
        GameObject fireSystemObject = Instantiate(fireParticleSystemPrefab, collisionPoint, Quaternion.identity);
        ParticleSystem fireSystemComponent = fireSystemObject.GetComponent<ParticleSystem>();
        SetParticleSystemShape(fireSystemComponent);
        activeParticleSystems.Add(fireSystemComponent);

        StartCoroutine(ScaleOverTimeAndDestroy(fireSystemObject, maxScale, scaleSpeed, particleSystemDuration));

        // To start the smoke after fire particle system
        yield return new WaitForSeconds(smokeDelay);
        
        GameObject smokeSystemObject = Instantiate(smokeParticleSystemPrefab, collisionPoint, Quaternion.identity);
        ParticleSystem smokeSystemComponent = smokeSystemObject.GetComponent<ParticleSystem>();
        SetParticleSystemShape(smokeSystemComponent);
        activeParticleSystems.Add(smokeSystemComponent);

        // To stop smoke at the same time as fire
        StartCoroutine(ScaleOverTimeAndDestroy(smokeSystemObject, maxScale, scaleSpeed, particleSystemDuration - smokeDelay));
    }

    private void SetParticleSystemShape(ParticleSystem particleSystemComponent)
    {
        ParticleSystem.ShapeModule shapeModule = particleSystemComponent.shape;
        shapeModule.shapeType = ParticleSystemShapeType.Mesh;
        shapeModule.mesh = particleModelPrefab.GetComponent<MeshFilter>().sharedMesh;
    }

    private void StopParticleSystemAtPoint(Vector3 collisionPoint)
    {
        List<ParticleSystem> systemsToRemove = new List<ParticleSystem>();
        foreach (ParticleSystem ps in activeParticleSystems)
        {
            float distance = Vector3.Distance(ps.transform.position, collisionPoint);
            if (distance < ps.transform.localScale.x * 2f)
            {
                ps.Stop();
                systemsToRemove.Add(ps);
            }
        }
        foreach (ParticleSystem ps in systemsToRemove)
        {
            activeParticleSystems.Remove(ps);
        }
    }

    private IEnumerator ScaleOverTimeAndDestroy(GameObject particleSystemObject, float targetScale, float speed, float duration)
    {
        float timer = 0;
        Vector3 initialScale = particleSystemObject.transform.localScale;
        Vector3 targetScaleVector = new Vector3(targetScale, targetScale, targetScale);

        while (particleSystemObject.transform.localScale != targetScaleVector)
        {
            float currentScale = Mathf.Lerp(1, targetScale, timer / speed);
            particleSystemObject.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            if (timer / speed >= 1.0f) break; // Prevent overshooting
            timer += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(duration);

        ParticleSystem ps = particleSystemObject.GetComponent<ParticleSystem>();
        activeParticleSystems.Remove(ps);
        Destroy(particleSystemObject);
    }
}