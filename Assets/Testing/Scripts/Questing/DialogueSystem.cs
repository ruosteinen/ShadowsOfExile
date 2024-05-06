using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance { get; set; }

    public TextMeshProUGUI questText;

    public Button acceptBTN;
    public Button cancelBTN;

    public Canvas questUI;

    public bool questUIActive;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void openQuestUI()
    {
        questUI.gameObject.SetActive(true);
        questUIActive = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void closeQuestUI()
    {
        questUI.gameObject.SetActive(false);
        questUIActive = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
