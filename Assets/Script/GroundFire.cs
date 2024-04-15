using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroundFire : MonoBehaviour
{
    public GameObject fireParticleSystemPrefab;
    public GameObject smokeParticleSystemPrefab;
    public GameObject particleModelPrefab;
    public float maxScale = 4f;
    public float scaleSpeed = 2f;
    public float particleSystemDuration = 10f;
    public float smokeDelay = 1.0f;

    private List<ParticleSystem> activeFireParticleSystems = new List<ParticleSystem>();
    private Dictionary<ParticleSystem, ParticleSystem> fireToSmokeMap = new Dictionary<ParticleSystem, ParticleSystem>();

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
            StopFireAndSmokeAtPoint(collisionPoint);
        }
    }

   private IEnumerator StartFireAndSmokeSystems(Vector3 collisionPoint)
{
    GameObject fireSystemObject = Instantiate(fireParticleSystemPrefab, collisionPoint, Quaternion.identity);
    ParticleSystem fireSystemComponent = fireSystemObject.GetComponent<ParticleSystem>();
    SetParticleSystemShape(fireSystemComponent);
    activeFireParticleSystems.Add(fireSystemComponent);

    fireSystemObject.transform.localScale = Vector3.one * 0.1f;

    float startTime = Time.time;
    Vector3 initialScale = fireSystemObject.transform.localScale;

    while (Time.time - startTime < scaleSpeed)
    {
        float t = (Time.time - startTime) / scaleSpeed;

        float currentScale = Mathf.Lerp(initialScale.x, maxScale, t);
        fireSystemObject.transform.localScale = Vector3.one * currentScale;

        yield return null;
    }

    yield return new WaitForSeconds(smokeDelay);

    if (activeFireParticleSystems.Contains(fireSystemComponent))
    {
        GameObject smokeSystemObject = Instantiate(smokeParticleSystemPrefab, collisionPoint, Quaternion.identity);
        ParticleSystem smokeSystemComponent = smokeSystemObject.GetComponent<ParticleSystem>();
        SetParticleSystemShape(smokeSystemComponent);
        activeFireParticleSystems.Add(smokeSystemComponent);
        fireToSmokeMap.Add(fireSystemComponent, smokeSystemComponent);

        startTime = Time.time;
        initialScale = smokeSystemObject.transform.localScale;

        while (Time.time - startTime < scaleSpeed)
        {
            float t = (Time.time - startTime) / scaleSpeed;

            float currentScale = Mathf.Lerp(initialScale.x, maxScale, t);
            smokeSystemObject.transform.localScale = Vector3.one * currentScale;

            yield return null;
        }

        Destroy(smokeSystemObject, particleSystemDuration);
    }

    Destroy(fireSystemObject, particleSystemDuration);
}


    private void SetParticleSystemShape(ParticleSystem particleSystemComponent)
    {
        ParticleSystem.ShapeModule shapeModule = particleSystemComponent.shape;
        shapeModule.shapeType = ParticleSystemShapeType.Mesh;
        shapeModule.mesh = particleModelPrefab.GetComponent<MeshFilter>().sharedMesh;
    }

    private void StopFireAndSmokeAtPoint(Vector3 collisionPoint)
    {
        List<ParticleSystem> fireSystemsToRemove = new List<ParticleSystem>();
        foreach (var fireSystem in activeFireParticleSystems)
        {
            if (Vector3.Distance(fireSystem.transform.position, collisionPoint) < maxScale * 2f)
            {
                fireSystem.Stop();
                if (fireToSmokeMap.TryGetValue(fireSystem, out ParticleSystem smokeSystem))
                {
                    smokeSystem.Stop();
                    fireToSmokeMap.Remove(fireSystem);
                }
                fireSystemsToRemove.Add(fireSystem);
            }
        }

        foreach (var fireSystem in fireSystemsToRemove) activeFireParticleSystems.Remove(fireSystem);
    }
}