using UnityEngine;

public class LootHandler : MonoBehaviour
{
    public int resourceAmount = 0;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Loot")) resourceAmount++;
    }
}
