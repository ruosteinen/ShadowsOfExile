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

    TextMeshProUGUI questDialogText;

    Button acceptButton;
    TextMeshProUGUI acceptButtonText;

    Button rejectButton;
    TextMeshProUGUI rejectButtonText;

    public List<Quest> quests;
    public Quest currentActiveQuest = null;
    public int activeQuestIndex = 0;
    public bool firstTimeInteraction = true;
    public int currentDialog;

    private void Start()
    {
        questDialogText = DialogueSystem.Instance.questText;

        acceptButton = DialogueSystem.Instance.acceptBTN;
        acceptButtonText = DialogueSystem.Instance.acceptBTN.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();

        rejectButton = DialogueSystem.Instance.cancelBTN;
        rejectButtonText = DialogueSystem.Instance.cancelBTN.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
    }

    public void StartConversation()
    {
        isTalkingWithPlayer = true;

        // Interacting with the NPC for the first time
        if (firstTimeInteraction)
        {
            firstTimeInteraction = false;
            currentActiveQuest = quests[activeQuestIndex];
            StartQuestInitialDialog();
            currentDialog = 0;
        }
        else // Interacting with the NPC after the first time
        {

        } 
        
    }

    private void StartQuestInitialDialog()
    {
        DialogueSystem.Instance.openQuestUI();

        questDialogText.text = currentActiveQuest.info.initialDialog[currentDialog];
        acceptButtonText.text = "Next";
        acceptButton.onClick.RemoveAllListeners();
        acceptButton.onClick.AddListener(() =>
        {
            currentDialog++;
            CheckIfDialogDone();
        });

        rejectButton.gameObject.SetActive(false);
    }

    private void CheckIfDialogDone()
    {
        if (currentDialog == currentActiveQuest.info.initialDialog.Count - 1) // If its the l
        {
            questDialogText.text = currentActiveQuest.info.initialDialog[currentDialog];

            currentActiveQuest.initialDialogCompleted = true;

            acceptButtonText.text = currentActiveQuest.info.acceptOption;
            acceptButton.onClick.RemoveAllListeners();
            acceptButton.onClick.AddListener(() =>
            {
                AcceptedQuest();
            });

            rejectButton.gameObject.SetActive(true);
            acceptButtonText.text = currentActiveQuest.info.declineOption;
            acceptButton.onClick.RemoveAllListeners();
            acceptButton.onClick.AddListener(() =>
            {
                DeclinedQuest();
            });
        }
        else //If there are more dialogs
        {
            questDialogText.text = currentActiveQuest.info.initialDialog[currentDialog];
            acceptButtonText.text = "Next";
            acceptButton.onClick.RemoveAllListeners();
            acceptButton.onClick.AddListener(() =>
            {
                currentDialog++;
                CheckIfDialogDone();
            });
        }
    }

    private void DeclinedQuest()
    {
        print("Quest Declined");
    }

    private void AcceptedQuest()
    {
        print("Quest Accepted");
    }

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
}
