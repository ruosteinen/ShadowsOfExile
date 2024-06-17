using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public MouseView mouseView; // Reference to the MouseView script
    public SpellShooting playerShooting; // Reference to the PlayerShooting script
    public GameObject crosshairUI;
    public GameObject healthUI;
    public GameObject manaUI;
    public GameObject questUI;
    public GameObject potionUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        mouseView.enabled = true; // Enable camera movement
        playerShooting.enabled = true; // Enable shooting
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        crosshairUI.SetActive(true);
        healthUI.SetActive(true);
        manaUI.SetActive(true);
        questUI.SetActive(true);
        potionUI.SetActive(true);
    }

    public void Restart()
    {
        // Assuming Restart should reset the scene
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        mouseView.enabled = false; // Disable camera movement
        playerShooting.enabled = false; // Disable shooting
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        crosshairUI.SetActive(false);
        healthUI.SetActive(false);
        manaUI.SetActive(false);
        questUI.SetActive(false);
        potionUI.SetActive(false);
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        Debug.Log("Loading menu...");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
