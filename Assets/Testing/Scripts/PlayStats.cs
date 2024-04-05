using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayStats : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float defense;
    [SerializeField] private TMP_Text healthValue;

    private float currentHealth;

    public HealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthText();

        healthBar.SetSliderMax(maxHealth);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount - defense;
        healthBar.SetSlider(currentHealth);
        UpdateHealthText();
    }

  
    private void UpdateHealthText()
    {
        healthValue.text = $"{currentHealth} / {maxHealth}";
    }

}