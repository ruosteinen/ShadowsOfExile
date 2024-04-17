using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth;
    public float health = 100;
    public float reduceSpeed = 2;

    public float target = 1;
    [SerializeField] private GameObject[] Drops;
    [SerializeField] private AudioClip[] droidSounds;
    [SerializeField] private AudioSource playerAudioSource;

    private AudioSource droidSource;
    private Canvas healthbarCanvas;
    private Image healthBarSprite;
    private Camera playerCam;

    public bool playerInvincible = false;

    private bool isEnemy;

    private void Start()
    {
        health = maxHealth;
        isEnemy = gameObject.CompareTag("Enemy");
        droidSource = gameObject.GetComponent<AudioSource>();

        if (isEnemy)
        {
            healthbarCanvas = GetComponentInChildren<Canvas>();
            healthBarSprite = gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
            playerCam = Camera.main;

            healthBarSprite.fillAmount = health / maxHealth;
        }
    }

    private void Update()
    {
        if(isEnemy && healthbarCanvas.enabled)
         {
             if(Vector3.Distance(transform.position, playerCam.transform.position) > 5)
                 healthbarCanvas.transform.rotation = Quaternion.LookRotation(transform.position - (playerCam.transform.position + new Vector3(0, 0.449999988f, 0.216999993f)));
             else
             {
                 healthbarCanvas.transform.rotation = Quaternion.LookRotation(transform.position - (playerCam.transform.position - new Vector3(0, 1, 0)));
             }
             healthBarSprite.fillAmount = Mathf.MoveTowards(healthBarSprite.fillAmount, target, reduceSpeed * Time.deltaTime);

            if (health <= 0)
            {
                KillEnemy();
            }
        }
    }

    private void FreezeConstrains()
    {
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    }

    private void UnFreezePos()
    {
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void TakeDamage(int damage)
    {
        if (gameObject.CompareTag("Player") && !playerInvincible || isEnemy)
        {
            health -= damage;
        }

        UpdateHealthBar();

        if(health < 0)
        {
            health = 0;
        }
    }

    private void UpdateHealthBar()
    {
        target = health / maxHealth;
    }

    private void KillEnemy()
    {
        if (Drops.Length > 0)
        {
            int n = Random.Range(1, 3);
            for (int i = 0; i < n; i++)
            {
                Instantiate(Drops[0], gameObject.transform.position, Quaternion.identity);
            }
        }
        else
        {
            Debug.LogError("Drops array does not have any elements.");
        }



        // Destroy the enemy GameObject
        Destroy(gameObject);
    }
}