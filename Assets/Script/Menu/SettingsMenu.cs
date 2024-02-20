using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Dropdown resolutionDropdown;
    public Dropdown qualityDropdown;
    public Button fullscreenButton;
    public TextMeshProUGUI fullscreenButtonText;
    public Slider mouseSensitivitySlider;
    Resolution[] resolutions;

    void Start()
    {
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        resolutions = Screen.resolutions;
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height + " " + resolutions[i].refreshRate + "Hz";
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        LoadSettings(currentResolutionIndex);
        
        fullscreenButtonText.text = "Fullscreen";
        
        
        mouseSensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 50f);
        mouseSensitivitySlider.onValueChanged.AddListener(SetMouseSensitivity);
    }


    public void SetMouseSensitivity(float sensitivity)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity);
        PlayerPrefs.Save();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        //SaveSettings();
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex, true); 
        //SaveSettings();
    }

    public void ToggleFullscreenMode()
    {
        Screen.fullScreen = !Screen.fullScreen;
        UpdateFullscreenButtonText();
        //SaveSettings();
    }

    public void ExitSettings()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("QualitySettingPreference", qualityDropdown.value);
        PlayerPrefs.SetInt("ResolutionPreference", resolutionDropdown.value);
        PlayerPrefs.SetInt("FullscreenPreference", System.Convert.ToInt32(Screen.fullScreen));
    }

    public void LoadSettings(int currentResolutionIndex)
    {
        qualityDropdown.value = PlayerPrefs.GetInt("QualitySettingPreference", 3);
        resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionPreference", currentResolutionIndex);
        Screen.fullScreen = System.Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenPreference", System.Convert.ToInt32(Screen.fullScreen)));
        
        UpdateFullscreenButtonText();
    }

    private void UpdateFullscreenButtonText()
    {
        fullscreenButtonText.text = Screen.fullScreen ? "Windowed" : "Fullscreen";
    }
}
