using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayStats : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float defense;
    [SerializeField] private TMP_Text healthValue;
    [SerializeField] private float maxMana;
    [SerializeField] private TMP_Text manaValue;

    private float currentHealth;
    public float currentMana;
    private float manaRegenAmount = 5f;

    public HealthBar healthBar;
    public ManaBar manaBar;

    void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        UpdateHealthText();
        UpdateManaText();
        healthBar.SetSliderMax(maxHealth);
        manaBar.SetSliderMax(maxMana);
    }

    void Update()
    {
        RegenerateMana();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount - defense;
        healthBar.SetSlider(currentHealth);
        UpdateHealthText();
    }

    public void ManaCast(float amount)
    {
        currentMana -= amount;
        manaBar.SetSlider(currentMana);
        UpdateManaText();
    }

    private void RegenerateMana()
    {
        currentMana = Mathf.Clamp(currentMana + manaRegenAmount * Time.deltaTime, 0f, maxMana);
        manaBar.SetSlider(currentMana);
        UpdateManaText();
    }

    private void UpdateHealthText()
    {
        healthValue.text = $"{currentHealth} / {maxHealth}";
    }

    private void UpdateManaText()
    {
        manaValue.text = $"{currentMana} / {maxMana}";
    }
}
