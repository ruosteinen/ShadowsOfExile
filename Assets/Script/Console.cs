using UnityEngine;
using TMPro;

public class Console : MonoBehaviour
{
    public TMP_InputField consoleInputField;
    public PlayerQ3LikeController playerController;
    void Start()
    {
        if (consoleInputField == null)
        {
            Debug.LogError("Console InputField component is not assigned.");
            return;
        }

        consoleInputField.onEndEdit.AddListener(ExecuteCommand);
        consoleInputField.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            consoleInputField.gameObject.SetActive(!consoleInputField.gameObject.activeSelf);
            if (consoleInputField.gameObject.activeSelf)
            {
                consoleInputField.ActivateInputField();
            }
        }
    }

    void ExecuteCommand(string commandInput)
    {

        string[] parts = commandInput.Split(' ');
        if (parts.Length > 0)
        {
            string command = parts[0].ToLower();
            float value;

            switch (command)
            {
                case "gravity":
                    if (parts.Length > 1 && float.TryParse(parts[1], out value))
                    {
                        if (playerController != null)
                        {
                            playerController.useCustomGravity = true;
                            playerController.customGravity = value;
                            Debug.Log("new gravity");
                        }
                    }
                    else
                    {
                        Debug.LogError("Command not found.");
                    }
                    break;

                case "armor_weight":
                    if (parts.Length > 1 && float.TryParse(parts[1], out value))
                    {
                        if (playerController.armor != null)
                        {
                            playerController.armor.weight = value;
                            Debug.Log("IDI NAKHUI!!!!!!!!!!");
                        }
                    }
                    else
                    {
                        Debug.LogError("Command not found.");
                    }
                    break;
            }
        }

        consoleInputField.text = "";
        consoleInputField.gameObject.SetActive(false);
    }
}
