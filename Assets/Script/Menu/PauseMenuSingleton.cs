using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class PauseMenuSingleton : MonoBehaviour
{
    public static PauseMenuSingleton Instance { get; private set; }

    public GameObject pauseMenuPanel;
    public GameObject settingsPanel;

    private bool _isPaused;  //false by default

    public bool IsPaused
    {
        get { return _isPaused; }
       set { _isPaused = value; }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
            //DontDestroyOnLoad(gameObject); // If we need to save the pause menu between scenes (no fucking need)
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f; //Stop game time
        IsPaused = true;

        //Unlocking the cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        Time.timeScale = 1f; //Resuming game time
        IsPaused = false;

        //Cursor lock
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void LoadSettingsMenu()
    {
        //Time.timeScale = 1f; //Resuming game time
        //SceneManager.LoadScene("SettingsMenu");
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }
    public void LoadMenu()
    {
        Time.timeScale = 1f; //Resuming game time
        SceneManager.LoadScene("MainMenu");
    }
}
