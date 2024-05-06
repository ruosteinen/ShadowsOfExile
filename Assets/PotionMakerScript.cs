using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PotionMakerScript : MonoBehaviour
{
    public string interactionText = "Press U to use"; 
    public int fontSize = 30;
    public GameObject potionMakerScreen;
    private bool playerInRange;

    public TMP_Text RequiredAmountText;
    public TMP_Text CurrentAmountText;

    public bool isPotionMaking = false;
    
    public int potionAmount = 0;
    private void Start()
    {
        string reqText = "You need 2 resources to create a potion";
        RequiredAmountText.text = reqText;
    }

    private void Update()
    {
        if (playerInRange)
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                isPotionMaking = true;
                potionMakerScreen.SetActive(true);
                Time.timeScale = 0f;
                PlayerQ3LikeController playerController = FindObjectOfType<PlayerQ3LikeController>();
                if (playerController != null && playerController.Crosshair != null)
                {
                    playerController.Crosshair.gameObject.SetActive(false);
                }
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                PauseMenuSingleton.Instance.IsPaused = true;
            }
        }
        
        LootHandler lootHandler = FindObjectOfType<LootHandler>();

        if (lootHandler != null)
        {
            CurrentAmountText.text = "Current amount: " + lootHandler.resourceAmount;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = false;
    }

    public void ClosePotionMaker()
    {
        isPotionMaking = false; 
        PauseMenuSingleton.Instance.Resume();
        potionMakerScreen.SetActive(false);   
    }

    public void MakePotion()
    {
        Debug.Log("MakePotion called");
        LootHandler lootHandler = FindObjectOfType<LootHandler>();
        if (lootHandler != null  && lootHandler.resourceAmount >= 2) {
            lootHandler.resourceAmount -= 2;
            potionAmount++;
            Debug.Log(lootHandler.resourceAmount);
        } else {
            Debug.Log("LootHandler not found");
        }
    }

    public void PlayGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("SampleScene");
    }
    
    private void OnGUI()
    {
        if (playerInRange && !isPotionMaking)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = fontSize;
            GUI.Label(new Rect(screenPosition.x - 100, Screen.height - screenPosition.y - 50, 400, 40), interactionText, style);
        }
    }
}
