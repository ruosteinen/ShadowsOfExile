using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DarkCrystalManager : MonoBehaviour
{
    public int crystalHealth = 100; // Initial health of the crystal
    public TextMeshProUGUI message1Text; // Reference to the first message UI Text
    public TextMeshProUGUI message2Text; // Reference to the second message UI Text

    private void Start()
    {
        // Initialize UI messages
        message1Text.text = "Find the Dark Crystal in the cave in the Forest and Destroy it!";
        message2Text.text = "";
    }

    // Call this method when the crystal takes damage
    public void TakeDamage(int damage)
    {
        crystalHealth -= damage;

        if (crystalHealth <= 0)
        {
            crystalHealth = 0;
            DestroyCrystal();
        }
    }

    private void DestroyCrystal()
    {
        // Display the second message when the crystal is destroyed
        message1Text.text = "";
        message2Text.text = "More in the next update";
    }
}
