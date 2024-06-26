using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public int level;
    public float currentXp;
    public float requiredXp;

    private float lerpTimer;
    private float delayTimer;
    [Header("UI")]
    public Image FrontExpBar;
    public Image BackExpBar;
    public TextMeshProUGUI Level;
    public TextMeshProUGUI Exp;
    [Header("Multipliers")]
    [Range(1f, 300f)]
    public float additionMultiplier = 300;
    [Range(2f, 4f)]
    public float powerMultiplier = 2;
    [Range(7f, 14f)]
    public float divisionMultiplier = 7;

    
    public PotionMakerScript potionMakerScript;
    public GameObject GameOverScreen;
    void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        UpdateHealthText();
        UpdateManaText();
        healthBar.SetSliderMax(maxHealth);
        manaBar.SetSliderMax(maxMana);
        FrontExpBar.fillAmount = currentXp / requiredXp;
        BackExpBar.fillAmount = currentXp / requiredXp;
        requiredXp = CalculateRequiredXP();
        Level.text = "Level " + level;
    }

    void Update()
    {
        RegenerateMana();
        UpdateXpUI();
        if (Input.GetKeyDown(KeyCode.E))
            GainExperienceFlatRate(2000);
        if (currentXp > requiredXp)
            LevelUp();
        
        
        if (Input.GetKeyDown(KeyCode.P) && potionMakerScript.potionAmount >= 1)
        {
            if (currentHealth < maxHealth)
            {
                Heal(30);
                potionMakerScript.potionAmount--;
            }
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth += (amount - defense);
        healthBar.SetSlider(currentHealth);
        UpdateHealthText();
        
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

    private void UpdateManaText()
    {
        manaValue.text = $"{(int)currentMana} / {(int)maxMana}";
    }

    public void UpdateXpUI()
    {
        float xpFraction = currentXp / requiredXp;
        float FXP = FrontExpBar.fillAmount;
        if (FXP < xpFraction)
        {
            delayTimer += Time.deltaTime;
            BackExpBar.fillAmount = xpFraction;
            if (delayTimer > 3)
            {
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / 4;
                FrontExpBar.fillAmount = Mathf.Lerp(FXP, BackExpBar.fillAmount, percentComplete);
            }
        }
        Exp.text = currentXp + "/" + requiredXp;
    }

    public void GainExperienceFlatRate(int xpGained)
    {
        currentXp += xpGained;
        lerpTimer = 0f;
        delayTimer = 0f;
    }

    public void GainExperienceScaleble(int xpGained, int passedLevel)
    {
        if (passedLevel < level)
        {
            float multiplier = 1 + (level - passedLevel) * 0.1f;
            currentXp += xpGained * multiplier;
        }
        else
        {
            currentXp += xpGained;
        }
        lerpTimer = 0f;
        delayTimer = 0f;
    }

    public void LevelUp()
    {
        level++;
        currentHealth *= 1.2f;
        currentMana *= 1.2f;
        maxHealth *= 1.2f;
        maxMana *= 1.2f;
        attack *= 1.2f;
        defense *= 1.2f;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        currentMana = Mathf.Min(currentMana, maxMana);
        healthBar.SetSliderMax(maxHealth);
        manaBar.SetSliderMax(maxMana);
        FrontExpBar.fillAmount = 0f;
        BackExpBar.fillAmount = 0f;
        currentXp = Mathf.RoundToInt(currentXp - requiredXp);
        requiredXp = CalculateRequiredXP();
        Level.text = "Level " + level;
        UpdateHealthText();
        UpdateManaText();
    }

    private int CalculateRequiredXP()
    {
        int SolveForRequiredXp = 0;
        for (int levelCycle = 1; levelCycle <= level; levelCycle++)
        {
            SolveForRequiredXp += (int)Mathf.Floor(levelCycle + additionMultiplier * Mathf.Pow(powerMultiplier, levelCycle / divisionMultiplier));
        }
        return SolveForRequiredXp / 4;
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
