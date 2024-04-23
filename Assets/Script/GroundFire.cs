using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroundFire : MonoBehaviour
{
    public GameObject fireParticleSystemPrefab;
    public GameObject smokeParticleSystemPrefab;
    public GameObject burntAreaPrefab;
    public float maxScale = 4f;
    public float scaleSpeed = 2f;
    public float particleSystemDuration = 10f;
    public float smokeDelay = 1.0f;

    private List<ParticleSystem> activeFireParticleSystems = new List<ParticleSystem>();
    private Dictionary<ParticleSystem, ParticleSystem> fireToSmokeMap = new Dictionary<ParticleSystem, ParticleSystem>();
    private Dictionary<ParticleSystem, GameObject> fireToVisualMap = new Dictionary<ParticleSystem, GameObject>();

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
        GameObject fireVisualObject = CreateVisualEffect(collisionPoint, burntAreaPrefab);
        GameObject fireSystemObject = Instantiate(fireParticleSystemPrefab, collisionPoint, Quaternion.identity);
        ParticleSystem fireSystemComponent = fireSystemObject.GetComponent<ParticleSystem>();

        SetParticleSystemShape(fireSystemComponent, fireVisualObject);
        activeFireParticleSystems.Add(fireSystemComponent);
        fireToVisualMap.Add(fireSystemComponent, fireVisualObject);

        yield return ScaleVisualEffectOverTime(fireVisualObject, maxScale);

        yield return new WaitForSeconds(smokeDelay);

        if (activeFireParticleSystems.Contains(fireSystemComponent))
        {
            GameObject smokeSystemObject = Instantiate(smokeParticleSystemPrefab, collisionPoint, Quaternion.identity);
            ParticleSystem smokeSystemComponent = smokeSystemObject.GetComponent<ParticleSystem>();
            
            SetParticleSystemShape(smokeSystemComponent, fireVisualObject);
            activeFireParticleSystems.Add(smokeSystemComponent);
            fireToSmokeMap.Add(fireSystemComponent, smokeSystemComponent);

            yield return ScaleVisualEffectOverTime(fireVisualObject, maxScale);

            Destroy(smokeSystemObject, particleSystemDuration);
        }

        Destroy(fireSystemObject, particleSystemDuration);
        Destroy(fireVisualObject, particleSystemDuration);
    }

    private GameObject CreateVisualEffect(Vector3 collisionPoint, GameObject prefab)
    {
        GameObject visualEffectObject = Instantiate(prefab, collisionPoint, Quaternion.identity);
        visualEffectObject.transform.localScale = new Vector3(0.1f, 1f, 0.1f); // Start small, but keep original Y scale
        return visualEffectObject;
    }

    private IEnumerator ScaleVisualEffectOverTime(GameObject visualEffectObject, float targetScale)
    {
        float startTime = Time.time;
        Vector3 initialScale = visualEffectObject.transform.localScale;
        float initialYScale = initialScale.y; // Save the initial Y scale

        while (Time.time - startTime < scaleSpeed)
        {
            float t = (Time.time - startTime) / scaleSpeed;
            float currentScale = Mathf.Lerp(0.1f, targetScale, t); // Lerp from 0.1f to targetScale for X and Z
            visualEffectObject.transform.localScale = new Vector3(currentScale, 0.15f, currentScale);
            yield return null;
        }
    }

    private void StopFireAndSmokeAtPoint(Vector3 collisionPoint)
    {
        List<ParticleSystem> fireSystemsToRemove = new List<ParticleSystem>();

        foreach (var fireSystem in activeFireParticleSystems)
        {
            GameObject visualEffectObject = fireToVisualMap[fireSystem];
            float currentScale = visualEffectObject.transform.localScale.x;

            if (Vector3.Distance(visualEffectObject.transform.position, collisionPoint) < currentScale * 1.5f)
            {
                fireSystem.Stop();
                if (fireToSmokeMap.TryGetValue(fireSystem, out ParticleSystem smokeSystem))
                {
                    smokeSystem.Stop();
                    fireToSmokeMap.Remove(fireSystem);
                }

                fireSystemsToRemove.Add(fireSystem);
                Destroy(visualEffectObject); 
                fireToVisualMap.Remove(fireSystem);
            }
        }

        foreach (var fireSystemToRemove in fireSystemsToRemove) activeFireParticleSystems.Remove(fireSystemToRemove);
        
    } 
    private void SetParticleSystemShape(ParticleSystem particleSystemComponent, GameObject visualPrefab)
    {
        ParticleSystem.ShapeModule shapeModule = particleSystemComponent.shape;
        shapeModule.shapeType = ParticleSystemShapeType.Mesh;
     
        MeshFilter meshFilter = visualPrefab.GetComponent<MeshFilter>();
        if (meshFilter != null) shapeModule.mesh = meshFilter.sharedMesh;
        
        else
        {
            MeshRenderer meshRenderer = visualPrefab.GetComponent<MeshRenderer>();
            if (meshRenderer != null) shapeModule.meshRenderer = meshRenderer;
            
            else Debug.LogWarning("SetParticleSystemShape: No MeshFilter or MeshRenderer found on visualPrefab.");
        }
    }
}