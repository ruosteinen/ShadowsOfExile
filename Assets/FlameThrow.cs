using UnityEngine;

public class FlameThrow : MonoBehaviour
{
    public GameObject fireballPrefab;
    public float fireBallSpeed; 
    public float spawnDistance = 1f;
    public Camera playerCamera;
    public PlayerQ3LikeController playerController;

    private void Update()
    {
        if ( playerController.fireSpellInUse && Input.GetMouseButtonDown(0) && playerController.mana > 1f)
        {
            if (playerCamera != null && !PauseMenuSingleton.Instance.IsPaused)
            {
                Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
                Ray ray = playerCamera.ScreenPointToRay(screenCenter);
                Vector3 spawnPoint = ray.origin + ray.direction * spawnDistance;
                GameObject fireBall = Instantiate(fireballPrefab, spawnPoint, Quaternion.identity);

                Rigidbody fireBallRB = fireBall.GetComponent<Rigidbody>();
                if (fireBallRB != null)
                {
                    Vector3 playerVel = playerController.playerVelocity;
                    fireBallRB.velocity = (ray.direction * fireBallSpeed) + playerVel;
                    playerController.mana --;
                }
            }
        }
    }
   

    /*private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Flammable"))
        {
            Flammable flammable = collision.gameObject.GetComponentInChildren<Flammable>();
            if (flammable != null && !flammable.isOnFire) flammable.Ignite();
        }
    }*/
}






/*using UnityEngine;

public class FlameThrow : MonoBehaviour
{
    //public ParticleSystem flamethrow_FX;
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) flamethrow_FX.Play();
        else if (Input.GetMouseButtonUp(0)) flamethrow_FX.Stop();
    }

    private void OnParticleCollision(GameObject obj)
    {
        if(obj.tag == "Flammable")
        {
            if(!obj.transform.GetChild(0).GetComponent<Flammable>().isOnFire) obj.transform.GetChild(0).GetComponent<Flammable>().Ignite();
        }
    }
}*/
