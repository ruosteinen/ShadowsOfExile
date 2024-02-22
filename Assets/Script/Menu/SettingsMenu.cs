using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Dropdown resolutionDropdown;
    [SerializeField] private Dropdown qualityDropdown;
    [SerializeField] private Button fullscreenButton;
    [SerializeField] private TextMeshProUGUI fullscreenButtonText;
    [SerializeField] private Slider mouseSensitivitySlider;
    private Resolution[] _resolutions;

    void Start()
    {
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        _resolutions = Screen.resolutions;
        int currentResolutionIndex = 0;

        for (int i = 0; i < _resolutions.Length; i++)
        {
            string option = _resolutions[i].width + "x" + _resolutions[i].height + " " + _resolutions[i].refreshRateRatio+ "Hz";
            options.Add(option);
            if (_resolutions[i].width == Screen.currentResolution.width && _resolutions[i].height == Screen.currentResolution.height)
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


    private void SetMouseSensitivity(float sensitivity)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity);
        PlayerPrefs.Save();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = _resolutions[resolutionIndex];
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
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true); 
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("QualitySettingPreference", qualityDropdown.value);
        PlayerPrefs.SetInt("ResolutionPreference", resolutionDropdown.value);
        PlayerPrefs.SetInt("FullscreenPreference", System.Convert.ToInt32(Screen.fullScreen));
    }

    private void LoadSettings(int currentResolutionIndex)
    {
        int highQualityIndex = 0;
        
        qualityDropdown.value = PlayerPrefs.GetInt("QualitySettingPreference", highQualityIndex);
        
        resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionPreference", currentResolutionIndex);
        Screen.fullScreen = System.Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenPreference", System.Convert.ToInt32(Screen.fullScreen)));

        UpdateFullscreenButtonText();
        
        QualitySettings.SetQualityLevel(qualityDropdown.value, true);
    }

    private void UpdateFullscreenButtonText()
    {
        fullscreenButtonText.text = Screen.fullScreen ? "Windowed" : "Fullscreen";
    }
}
