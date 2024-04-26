using UnityEngine;

public class PotionMakerScript : MonoBehaviour
{
    public string interactionText = "Press E to use"; 
    public int fontSize = 20;
    public GameObject potionMakerScreen;
    private bool playerInRange; 

    private void Update()
    {
        if (playerInRange)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
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
        PauseMenuSingleton.Instance.IsPaused = false;
        potionMakerScreen.SetActive(false);   
    }

    private void OnGUI()
    {
        if (playerInRange)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = fontSize;
            GUI.Label(new Rect(screenPosition.x - 100, Screen.height - screenPosition.y - 50, 400, 40), interactionText, style);
        }
    }
}