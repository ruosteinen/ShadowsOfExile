using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaShootingTest: MonoBehaviour
{
    public GameObject projectilePrefabLeft;
    public GameObject projectilePrefabRight;
    public float shootForce = 10f;
    public float manaCostLeft = 10f;
    public float manaCostRight = 20f;
    public PlayStats playerStats;

    void Start()
    {
        playerStats = FindObjectOfType<PlayStats>();

        if (playerStats == null)
        {
            Debug.LogError("No PlayStats component found in the scene.");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && playerStats != null && playerStats.currentMana >= manaCostLeft)
        {
            Shoot(projectilePrefabLeft, manaCostLeft);
        }

        if (Input.GetMouseButtonDown(1) && playerStats != null && playerStats.currentMana >= manaCostRight)
        {
            Shoot(projectilePrefabRight, manaCostRight);
        }
    }

    void Shoot(GameObject prefab, float manaCost)
    {
        GameObject projectile = Instantiate(prefab, transform.position, transform.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(transform.forward * shootForce, ForceMode.Impulse);
        }
        else
        {
            Debug.LogWarning("Projectile prefab does not contain Rigidbody component!");
        }

        playerStats.ManaCast(manaCost);
    }
}


