using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30f;
    public float bulletPrefabLifeTime = 3f;
    public Camera playerCamera;
    public PlayStats playerStats; // Reference to PlayStats
    public float manaCost = 10f;

    void Update()
    {
        // Check if the player is alive and has enough mana before shooting
        if (playerStats.isAlive && Input.GetKeyDown(KeyCode.Mouse0) && playerStats.currentMana >= manaCost)
        {
            FireWeapon();
        }
    }

    private void FireWeapon()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 shootDirection = playerCamera.transform.forward.normalized;
            rb.AddForce(shootDirection * bulletVelocity, ForceMode.Impulse);
            StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));
        }
        else
        {
            Debug.LogError("The bullet prefab does not have a Rigidbody component.");
        }
        playerStats.ManaCast(manaCost);
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}
