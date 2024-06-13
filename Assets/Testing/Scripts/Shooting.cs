using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    [Header("References")]
    public Transform cam;
    public Transform attackPoint;
    public GameObject projectilePrefab;
    public float shootForce = 20f;
    public KeyCode shootKey = KeyCode.Mouse0;

    private bool readyToShoot = true;
    public float shootCooldown = 1.0f;

    private void Update()
    {
        if (Input.GetKeyDown(shootKey) && readyToShoot)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        // Instantiate the projectile at the attack point
        GameObject projectile = Instantiate(projectilePrefab, attackPoint.position, cam.rotation);

        // Get the Rigidbody component of the projectile
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        // Calculate the shooting direction
        Vector3 forceDirection = cam.transform.forward;

        // Perform a raycast to see if we hit something
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        }

        // Apply force to the projectile
        Vector3 forceToAdd = forceDirection * shootForce;
        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

        // Implement shooting cooldown
        Invoke("ResetShot", shootCooldown);
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }
}