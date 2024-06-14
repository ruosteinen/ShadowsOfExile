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
    public PlayStats playerStats;
    public float manaCost = 10f;
    public float maxDistance = 50f;
    public int damage = 20;

    void Update()
    {
        if (PauseMenu.GameIsPaused)
        {
            return; // Do nothing if the game is paused
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && playerStats != null && playerStats.currentMana >= manaCost)
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

            WaterBall waterBall = bullet.GetComponent<WaterBall>();
            if (waterBall != null)
            {
                waterBall.maxDistance = maxDistance;
                waterBall.damage = damage;
                waterBall.Initialize(bulletSpawn.position);
            }
            else
            {
                Debug.LogError("The bullet prefab does not have a WaterBall component.");
            }

            StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));
        }
        else
        {
            Debug.LogError("The bullet prefab does not have a Rigidbody component.");
        }

        if (playerStats != null)
        {
            playerStats.ManaCast(manaCost);
        }
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}
