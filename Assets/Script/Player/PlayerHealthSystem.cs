using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealthSystem : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;

    private void Start() => currentHealth = maxHealth;

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthBar();
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Max(currentHealth - amount, 0);
        UpdateHealthBar();
        if (currentHealth <= 0) Die();
    }

    private void UpdateHealthBar() => healthBar.SetSlider(currentHealth);

    private void Die()
    {
        // Load the main menu scene
        //SceneManager.LoadScene("MainMenu");
        Debug.Log("Player died!");
    }
}
