using UnityEngine;
using System.Collections;
public class Flammable : MonoBehaviour
{
    public ParticleSystem fireFX;
    public ParticleSystem smokeFX;
    private Coroutine ignitionCoroutine;
    private float elapsedIgnitionTime; 
    public bool isOnFire;
    private bool wasExtinguished;
    public Material[] materials;
    public float colorChangeSpeed = 0.3f;

    // Specify ignition, smoke, and cooldown durations
    private readonly float ignitionDuration = 5f;
    private readonly float smokeDuration = 8f;
    private readonly float cooldownDuration = 2f;
    
    
    private void Start()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null) materials = meshRenderer.materials;
    }
    
    
    private void Update()
    {
        if (isOnFire)
        {
            foreach (Material mat in materials)
            {
                mat.color = Color.Lerp(mat.color, Color.black, Time.deltaTime * colorChangeSpeed);
            }
        }
    }
    
    private IEnumerator Ignition()
    {
        Debug.Log($"Ignition coroutine started for {gameObject.name}.");
        //gameObject.layer = LayerMask.NameToLayer("OnFire");

        while (elapsedIgnitionTime < ignitionDuration + smokeDuration + cooldownDuration)
        {
            if (elapsedIgnitionTime < ignitionDuration)
            {
                if (fireFX != null && !fireFX.isPlaying) fireFX.Play();
            }
            else if (elapsedIgnitionTime >= ignitionDuration && elapsedIgnitionTime < ignitionDuration + smokeDuration)
            {
                if (fireFX != null && !fireFX.isPlaying) fireFX.Play();
                if (smokeFX != null && !smokeFX.isPlaying) smokeFX.Play();
            }

            elapsedIgnitionTime += Time.deltaTime;
            yield return null;
        }

        // Once the burning process is complete, stop all effects and destroy the object
        if (fireFX != null && fireFX.isPlaying) fireFX.Stop();
        if (smokeFX != null && smokeFX.isPlaying) smokeFX.Stop();
        Destroy(gameObject);
    }

    public void Ignite()
    {
        if (!isOnFire)
        {
            isOnFire = true;
            ignitionCoroutine = StartCoroutine(Ignition());
        }
    }

    public void Extinguish()
    {
        if (isOnFire)
        {
            isOnFire = false;
            wasExtinguished = true;
            Debug.Log($"Extinguish called on {gameObject.name}.");

            if (ignitionCoroutine != null)
            {
                StopCoroutine(ignitionCoroutine);
                ignitionCoroutine = null;
            }

            // Stop the particle systems
            if (fireFX != null) fireFX.Stop();
            if (smokeFX != null) smokeFX.Stop();
        }
    }
}