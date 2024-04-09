using UnityEngine;

public class WaterBall : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.CompareTag("Flammable"))
        //{
           //Flammable flammable = collision.gameObject.GetComponent<Flammable>();
            //if (flammable != null && !flammable.isOnFire) flammable.Ignite();
            //Destroy(gameObject);
        //}
        Destroy(gameObject);
    }
}