using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellShooting : MonoBehaviour
{
    public GameObject bulletPrefab; // Corrected spelling
    public Transform bulletSpawn;
    public float bulletVelocity = 30f; // Added 'f' for float literal
    public float bulletPrefabLifeTime = 3f;
    public Camera playerCamera; // Reference to the player's camera
    public float manaCost = 15f;
    public PlayStats playerStats;

    // Update is called once per frame
    void Update()
    {
        playerStats = FindObjectOfType<PlayStats>();

        if (playerStats == null)
        {
            Debug.LogError("No PlayStats component found in the scene.");
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            FireWeapon();
        }
    }

    private void FireWeapon()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity); // Corrected spelling
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Use the forward direction of the camera for the bullet's velocity
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

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay) // Corrected spelling
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}
