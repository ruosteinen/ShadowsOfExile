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
            StartCoroutine(DestroyAfterSound());
        }
        else
        {
            Debug.LogError("Objective achieved audio source is null.");
            Destroy(gameObject);
        }

        // Display the second message when the crystal is destroyed
        message1Text.text = "";
        message2Text.text = "More in the next update";
    }

    private IEnumerator DestroyAfterSound()
    {
        // Wait until the audio has finished playing
        yield return new WaitForSeconds(objectiveAchievedAudioSource.clip.length);
        Destroy(gameObject);
    }
}
