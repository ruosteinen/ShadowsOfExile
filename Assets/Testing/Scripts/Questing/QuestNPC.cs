using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestNPC : MonoBehaviour
{
    public bool playerInRange;

    public bool isTalkingWithPlayer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void StartConversation()
    {
        isTalkingWithPlayer = true;
        print("Quest Discussion");

        DialogueSystem.Instance.openQuestUI();
        DialogueSystem.Instance.questText.text = "Find the dark cristal in the cave in the forest.";
        DialogueSystem.Instance.acceptBTN.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "I accept.";
        DialogueSystem.Instance.acceptBTN.onClick.AddListener(() =>
        {
            DialogueSystem.Instance.closeQuestUI();
            isTalkingWithPlayer = false;
        });
    }
}
