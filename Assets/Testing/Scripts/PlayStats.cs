using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayStats : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float defense;

    private float currentHealth;

    public HealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;

        healthBar.SetSliderMax(maxHealth);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount - defense;
        healthBar.SetSlider(currentHealth);
    }

}