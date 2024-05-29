using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DarkCrystalManager : MonoBehaviour
{
    public TextMeshProUGUI message1Text; // Reference to the first message UI Text
    public TextMeshProUGUI message2Text; // Reference to the second message UI Text

    [SerializeField] private float _maxHealth = 1000;
    private float _currentHealth;
    [SerializeField] private CrystalHealthBar _healthbar;

    private void Start()
    {
        // Initialize UI messages
        message1Text.text = "Find the Dark Crystal in the cave in the Forest and Destroy it!";
        message2Text.text = "";
        _currentHealth = _maxHealth;
        _healthbar.UpdateHealthBar(_maxHealth, _currentHealth);
    }

    // Call this method when the crystal takes damage
    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        _healthbar.UpdateHealthBar(_maxHealth, _currentHealth);

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            DestroyCrystal();
        }
    }

    private void DestroyCrystal()
    {
        // Display the second message when the crystal is destroyed
        message1Text.text = "";
        message2Text.text = "More in the next update";
        Destroy(gameObject);
    }
}
