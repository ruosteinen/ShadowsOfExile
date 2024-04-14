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

        StartCoroutine(ScaleParticleSystem(fireSystemObject, maxScale, scaleSpeed));

        yield return new WaitForSeconds(smokeDelay);

        if (activeFireParticleSystems.Contains(fireSystemComponent))
        {
            GameObject smokeSystemObject = Instantiate(smokeParticleSystemPrefab, collisionPoint, Quaternion.identity);
            ParticleSystem smokeSystemComponent = smokeSystemObject.GetComponent<ParticleSystem>();
            SetParticleSystemShape(smokeSystemComponent);
            activeFireParticleSystems.Add(smokeSystemComponent);
            fireToSmokeMap.Add(fireSystemComponent, smokeSystemComponent);
            StartCoroutine(ScaleParticleSystem(smokeSystemObject, maxScale, scaleSpeed));
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

        foreach (var fireSystem in fireSystemsToRemove)
        {
            activeFireParticleSystems.Remove(fireSystem);
        }
    }

    private IEnumerator ScaleParticleSystem(GameObject systemObject, float targetScale, float speed)
    {
        float timer = 0;
        Vector3 initialScale = systemObject.transform.localScale;
        Vector3 targetScaleVector = new Vector3(targetScale, targetScale, targetScale);

        while (systemObject.transform.localScale != targetScaleVector)
        {
            if (systemObject == null) yield break;
            float currentScale = Mathf.Lerp(initialScale.x, targetScale, timer / speed);
            systemObject.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            if (timer >= speed) break;
            timer += Time.deltaTime;
            yield return null;
        }
    }
}
