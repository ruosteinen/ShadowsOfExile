using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;

    [SerializeField] private GameObject mainMenuPanel;


    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
    
    public void ActivateSettingsMenu()
    {
        bool isSettingsActive = settingsPanel.activeSelf;

        if (!isSettingsActive)
        {
            settingsPanel.SetActive(true);
            mainMenuPanel.SetActive(false);
        }
        else
        {
            settingsPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        }
    }
    
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
}
