using System.Collections;
using TMPro;
using UnityEngine;

public class PlayStats : MonoBehaviour
{
    public float maxHealth;
    [SerializeField] private float defense;
    [SerializeField] public float attack;
    [SerializeField] private TMP_Text healthValue;
    public float maxMana;
    [SerializeField] private TMP_Text manaValue;

    private float currentHealth;
    public float currentMana;
    private float manaRegenAmount = 5f;

    public HealthBar healthBar;
    public ManaBar manaBar;

    public PotionMakerScript potionMakerScript;
    public GameObject GameOverScreen;
    private Animator animator;

    void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        UpdateHealthText();
        UpdateManaText();
        healthBar.SetSliderMax(maxHealth);
        manaBar.SetSliderMax(maxMana);
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        RegenerateMana();
        if (Input.GetKeyDown(KeyCode.P) && PotionMakerScript.potionAmount >= 1) 
        {
            if (currentHealth < maxHealth)
            {
                Heal(30);
                PotionMakerScript.potionAmount--; 
            }
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth += (amount - defense);
        healthBar.SetSlider(currentHealth);
        UpdateHealthText();
        animator.SetTrigger("TakeDamage");

        if (currentHealth <= 0) Die();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
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
        int displayedHealth = Mathf.Max((int)currentHealth, 0);
        healthValue.text = $"{displayedHealth} / {(int)maxHealth}";
    }

    private void UpdateManaText() => manaValue.text = $"{(int)currentMana} / {(int)maxMana}";
    private void Die() => StartCoroutine(DeathCoroutine());
    
    private IEnumerator DeathCoroutine()
    {
        animator.SetTrigger("Deth");
        GameOverScreen.SetActive(true);
        PauseMenuSingleton.Instance.IsPaused = true;
        yield return new WaitForSeconds(1f);
        OnDeathAnimationFinished();
    }
    
    public void OnDeathAnimationFinished()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }
}