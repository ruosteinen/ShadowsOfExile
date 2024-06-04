using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class GroundFire : MonoBehaviour
{
    public GameObject fireParticleSystemPrefab;
    public GameObject smokeParticleSystemPrefab;
    public GameObject burntAreaPrefab;
    public float maxScale = 4f;
    public float scaleSpeed = 2f;
    public float particleSystemDuration = 10f;
    public float smokeDelay = 1.0f;

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
            StopFireAndSmokeAtPoint(collisionPoint, 3f);
        }
    }

    private IEnumerator StartFireAndSmokeSystems(Vector3 collisionPoint)
    {
        GameObject fireVisualObject = CreateVisualEffect(collisionPoint, burntAreaPrefab);
        ParticleSystem fireSystemComponent = Instantiate(fireParticleSystemPrefab, collisionPoint, Quaternion.identity).GetComponent<ParticleSystem>();

        SetParticleSystemShape(fireSystemComponent, fireVisualObject);
        fireToVisualMap.Add(fireSystemComponent, fireVisualObject);

        yield return ScaleVisualEffectOverTime(fireVisualObject, maxScale);

        yield return new WaitForSeconds(smokeDelay);

        if (fireToVisualMap.ContainsKey(fireSystemComponent)) 
        {
            ParticleSystem smokeSystemComponent = Instantiate(smokeParticleSystemPrefab, collisionPoint, Quaternion.identity).GetComponent<ParticleSystem>();

            SetParticleSystemShape(smokeSystemComponent, fireVisualObject);

            yield return ScaleVisualEffectOverTime(fireVisualObject, maxScale);

            Destroy(smokeSystemComponent.gameObject, particleSystemDuration);
        }

        Destroy(fireSystemComponent.gameObject, particleSystemDuration);
        Destroy(fireVisualObject, particleSystemDuration);
    }

    private GameObject CreateVisualEffect(Vector3 collisionPoint, GameObject prefab)
    {
        GameObject visualEffectObject = Instantiate(prefab, collisionPoint, Quaternion.identity);
        visualEffectObject.transform.localScale = new Vector3(0.1f, 0f, 0.1f); // Start small, but keep original Y scale
        return visualEffectObject;
    }

    private IEnumerator ScaleVisualEffectOverTime(GameObject visualEffectObject, float targetScale)
    {
        float startTime = Time.time;
        Vector3 initialScale = visualEffectObject.transform.localScale;

        while (Time.time - startTime < scaleSpeed)
        {
            float t = (Time.time - startTime) / scaleSpeed;
            float currentScale = Mathf.Lerp(initialScale.x, targetScale, t); // Lerp from initial scale to targetScale for X and Z
            visualEffectObject.transform.localScale = new Vector3(currentScale, 0, currentScale); // Keep the original Y scale
            yield return null;
        }
    }

    private void StopFireAndSmokeAtPoint(Vector3 collisionPoint, float specificScale)
    {
        Collider[] colliders = Physics.OverlapBox(collisionPoint, new Vector3(specificScale/1.5f, specificScale/1.5f, specificScale/1.5f));

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("BurntAreaPrefab"))
            {
                GameObject visualEffectObject = collider.gameObject;

                if (fireToVisualMap.ContainsValue(visualEffectObject))
                {
                    ParticleSystem fireSystem = fireToVisualMap.FirstOrDefault(x => x.Value == visualEffectObject).Key;

                    if (fireSystem != null)
                    {
                        fireSystem.Stop();

                        if (fireToVisualMap.ContainsKey(fireSystem))
                        {
                            GameObject fireVisual = fireToVisualMap[fireSystem];
                            Destroy(fireVisual);
                            fireToVisualMap.Remove(fireSystem);
                        }
                    }
                }
            }
        }
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
