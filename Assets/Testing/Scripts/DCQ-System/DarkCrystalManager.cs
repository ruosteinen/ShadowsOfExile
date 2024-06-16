using System.Collections;
using UnityEngine;
using TMPro;

public class DarkCrystalManager : MonoBehaviour
{
    public TextMeshProUGUI message1Text; // Reference to the first message UI Text
    public TextMeshProUGUI message2Text; // Reference to the second message UI Text

    [SerializeField] private float _maxHealth = 1000;
    private float _currentHealth;
    [SerializeField] private CrystalHealthBar _healthbar;

    public AudioSource ambientAudioSource; // Reference to the ambient audio source
    public AudioSource objectiveAchievedAudioSource; // Reference to the objective achieved audio source
    public AudioSource winAudioSource; // Reference to the win audio source

    public GameObject winGameObject; // Reference to the GameObject to activate when the crystal is destroyed
    public GameObject cross;
    public GameObject health;
    public GameObject mana;
    public GameObject crystalCanvas;
    public GameObject potion;

    private void Start()
    {
        // Initialize UI messages
        message1Text.text = "Find the Dark Crystal in the cave in the Forest and Destroy it!";
        message2Text.text = "";
        _currentHealth = _maxHealth;
        _healthbar.UpdateHealthBar(_maxHealth, _currentHealth);

        // Play the ambient sound
        if (ambientAudioSource != null)
        {
            ambientAudioSource.Play();
        }

        // Ensure the win GameObject is initially inactive
        if (winGameObject != null)
        {
            winGameObject.SetActive(false);
        }
    }

    // Call this method when the crystal takes damage
    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            DestroyCrystal();
        }

        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        _healthbar.UpdateHealthBar(_maxHealth, _currentHealth);
    }

    private void DestroyCrystal()
    {
        // Stop the ambient sound
        if (ambientAudioSource != null)
        {
            ambientAudioSource.Stop();
            Debug.Log("Ambient audio stopped.");
        }

        // Play the objective achieved sound
        if (objectiveAchievedAudioSource != null)
        {
            objectiveAchievedAudioSource.Play();
            Debug.Log("Objective achieved audio is playing.");
        }
        else
        {
            Debug.LogError("Objective achieved audio source is null.");
        }

        // Display the second message when the crystal is destroyed
        message1Text.text = "";
        message2Text.text = "More in the next update";

        // Activate the win GameObject and play the win sound
        if (winGameObject != null)
        {
            winGameObject.SetActive(true);
            cross.SetActive(false);
            health.SetActive(false);
            mana.SetActive(false);
            crystalCanvas.SetActive(false);
            potion.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (winAudioSource != null)
            {
                winAudioSource.Play();
            }
            StartCoroutine(DestroyAfterSound());
        }
        else
        {
            Debug.LogError("Win GameObject is null.");
            Destroy(gameObject);
        }
    }

    private IEnumerator DestroyAfterSound()
    {
        // Wait until the longer of the objective achieved or win audio has finished playing
        float maxDuration = 0f;
        if (objectiveAchievedAudioSource != null)
        {
            maxDuration = Mathf.Max(maxDuration, objectiveAchievedAudioSource.clip.length);
        }
        if (winAudioSource != null)
        {
            maxDuration = Mathf.Max(maxDuration, winAudioSource.clip.length);
        }

        yield return new WaitForSeconds(maxDuration);
        Destroy(gameObject);
    }
}
