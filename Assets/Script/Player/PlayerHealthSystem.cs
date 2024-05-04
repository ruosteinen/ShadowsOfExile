using UnityEngine;

public class PlayerHealthSystem : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;
    public PotionMakerScript potionMakerScript;
    public GameObject GameOverScreen;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && potionMakerScript.potionAmount >= 1)
        {
            if (currentHealth < maxHealth)
            {
                Heal(30);
                potionMakerScript.potionAmount--;
            }
        }
    }

    private void Die()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PauseMenuSingleton.Instance.IsPaused = true;
        GameOverScreen.SetActive(true);
    }
}