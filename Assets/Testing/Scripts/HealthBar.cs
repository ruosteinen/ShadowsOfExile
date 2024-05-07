using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public Gradient gradient;
    public Image fill;
    public TextMeshProUGUI healthText;

    private int maxHealth = 100;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void SetSlider(float amount)
    {
        healthSlider.value = amount;
        fill.color = gradient.Evaluate(healthSlider.normalizedValue);
        currentHealth = (int)amount;
        UpdateHealthBar();
    }

    public void SetSliderMax(float amount)
    {
        healthSlider.maxValue = amount;
        SetSlider(amount);
    }
    private void UpdateHealthBar() => healthText.text = currentHealth.ToString();
}