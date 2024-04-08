using System.Collections;
using UnityEngine;

public class Flammable : MonoBehaviour
{
    public bool isOnFire;
    public ParticleSystem fireFX;
    public Material[] materials;
    public float colorChangeSpeed = 0.3f;
    public float destroyDelay = 5f;

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
        Debug.Log("Ignition coroutine started for " + gameObject.name);
        gameObject.tag = "Untagged";
        gameObject.layer = LayerMask.NameToLayer("OnFire");

        if (fireFX != null) fireFX.Play();
        else Debug.LogError("ParticleSystem component not found on " + gameObject.name);
        
        yield return new WaitForSeconds(5f);
        fireFX.Stop();
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}


/*
    // Debug.Log("Ignition coroutine started for " + gameObject.name);
    //isOnFire = true;
   // gameObject.tag = "Untagged"; // delete tag
   // gameObject.layer = LayerMask.NameToLayer("OnFire");
   // fireFX.Play();
   Debug.Log("Ignition coroutine started for " + gameObject.name);
   isOnFire = true;
   gameObject.tag = "Untagged"; // delete tag
   gameObject.layer = LayerMask.NameToLayer("OnFire");

   ParticleSystem fireFX = GetComponent<ParticleSystem>();
   if (fireFX != null)
   {
       fireFX.Play();
   }
   else
   {
       Debug.LogWarning("ParticleSystem component not found on " + gameObject.name);
   }
   
   
    yield return new WaitForSeconds(/*Random.Range(fireDelay - fireDelay/2 , fireDelay + fireDelay/2)* / 1f);
    // smokeFX.transform.parent = fireFX.transform.parent;
    fireFX.Stop();
    // yield return new WaitForSeconds(Random.Range(smokeDelay - smokeDelay/2 , smokeDelay + smokeDelay*2)); 
    // smokeFX.Stop(); 
    yield return new WaitForSeconds(destroyDelay);
    Destroy(gameObject);
 */