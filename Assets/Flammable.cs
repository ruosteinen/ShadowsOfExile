using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Flammable : MonoBehaviour
{
    
    public bool isOnFire;
    public ParticleSystem fireFX;
    //public ParticleSystem smokeFX;
    public Material[] materials;
    public float colorTime;
    public bool changeColorIsOn;
    public float colorChangeSpeed = 0.0003f;
    public float fireDelay = 20f;
    //public float smokeDelay = 20f;
    public float destroyDelay = 5f;
   // public int firePoints = 1; // 
   /*  private void Start()
    {
        fireFX = GetComponent<ParticleSystem>();
        //smokeFX = transform.GetChild(0).GetComponent<ParticleSystem>();
        materials = transform.parent.GetComponent<MeshRenderer>().materials;
        
        if(isOnFire) StartCoroutine(Ignition());
        
        //firePoints = Random.Range(firePoints - firePoints/2, firePoints + firePoints/2);
    }*/
   
   
   private void Start()
   {
       fireFX = GetComponent<ParticleSystem>();
       if (fireFX == null)
       {
           Debug.LogError("ParticleSystem не найден на объекте " + gameObject.name);
       }

       MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
       if (meshRenderer != null)
       {
           materials = meshRenderer.materials;
       }
       else
       {
           Debug.LogError("MeshRenderer не найден на объекте " + gameObject.name);
       }

       if (isOnFire) StartCoroutine(Ignition());
   }
   
   

    private void OnParticleCollision(GameObject obj)
    {
        if(obj.tag == "Flammable")
        {
            if(!obj.transform.GetChild(0).GetComponent<Flammable>().isOnFire) obj.transform.GetChild(0).GetComponent<Flammable>().Ignite();
        }
    }


   private void Update()
    {
        if (isOnFire)
        {
            colorTime += Time.deltaTime * colorChangeSpeed;
            foreach (Material mat in materials) mat.color = Color.Lerp(mat.color, Color.black, colorTime);
        }
    }
   
   
   public void Ignite()
   {
       //firePoints--;
       //if (firePoints == 0)
       Debug.Log("Ignite called on " + gameObject.name);
       StartCoroutine("Ignition");
   }
   
   IEnumerator Ignition()
   {
           Debug.Log("Ignition coroutine started for " + gameObject.name);
           isOnFire = true;
           gameObject.tag = "Untagged"; // delete tag
           gameObject.layer = LayerMask.NameToLayer("OnFire");
           fireFX.Play();
           yield return new WaitForSeconds(Random.Range(fireDelay - fireDelay/2 , fireDelay + fireDelay/2));
           // smokeFX.transform.parent = fireFX.transform.parent; // Эту строчку нужно удалить или изменить, так как smokeFX не используется.
           fireFX.Stop();
           // yield return new WaitForSeconds(Random.Range(smokeDelay - smokeDelay/2 , smokeDelay + smokeDelay*2)); // Эту строчку также нужно удалить или изменить.
           // smokeFX.Stop(); // Эту строчку также нужно удалить или изменить.
           yield return new WaitForSeconds(destroyDelay);
           //Destroy(gameObject);
   }
}
