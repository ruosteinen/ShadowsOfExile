using UnityEngine;
using UnityEngine.SceneManagement;
public class SettingsButton : MonoBehaviour
{ 
    public void LoadSettingsMenu()
    { 
        SceneManager.LoadScene("SettingsMenu");
    }
}