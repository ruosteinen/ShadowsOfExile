using System.Collections;
using UnityEngine;

public class Flammable : MonoBehaviour
{
    public bool isOnFire;
    public ParticleSystem fireFX;
    public ParticleSystem smokeFX;
    public Material[] materials;
    public float colorChangeSpeed = 0.3f;
    //public float destroyDelay = 5f;

    private void Start()
    {
        fireFX = GetComponentInChildren<ParticleSystem>();
        if (fireFX == null) Debug.LogError("ParticleSystem не найден на объекте " + gameObject.name);
        
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        
        if (meshRenderer != null) materials = meshRenderer.materials;
        
        else Debug.LogError("MeshRenderer не найден на объекте " + gameObject.name);
        
        if (isOnFire) Ignite();
    }

    private void OnParticleCollision(GameObject obj)
    {
        if (obj.CompareTag("Flammable"))
        {
            Flammable flammableComponent = obj.GetComponentInChildren<Flammable>();
            if (flammableComponent != null && !flammableComponent.isOnFire)
                flammableComponent.Ignite();
        }
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

    public void Ignite()
    {
        Debug.Log("Ignite called on " + gameObject.name);
        if (!isOnFire)
        {
            isOnFire = true;
            StartCoroutine(Ignition());
        }
    }

    IEnumerator Ignition()
    {
        //isOnFire = true;
        Debug.Log("Ignition coroutine started for " + gameObject.name);
        gameObject.tag = "Untagged";
        gameObject.layer = LayerMask.NameToLayer("OnFire");

        if (fireFX != null) fireFX.Play();
        else Debug.LogError("ParticleSystem component not found on " + gameObject.name);
        
        yield return new WaitForSeconds(5f);
        fireFX.Stop();
        smokeFX.Play();
        yield return new WaitForSeconds(8f); 
        smokeFX.Stop();
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}