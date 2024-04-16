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
        SetupResolutionDropdown();
        SetupQualityDropdown();
        SetupFullscreenText();
        SetupMouseSensitivitySlider();
    }

    private void SetupResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        _resolutions = Screen.resolutions;
        int currentResolutionIndex = 0;

        for (int i = 0; i < _resolutions.Length; i++)
        {
            string option = _resolutions[i].width + "x" + _resolutions[i].height + " " + _resolutions[i].refreshRate + "Hz";
            options.Add(option);

            if (_resolutions[i].width == Screen.currentResolution.width && 
                _resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionPreference", currentResolutionIndex);
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    private void SetupQualityDropdown()
    {
        qualityDropdown.ClearOptions();
        List<string> options = new List<string>(QualitySettings.names);
        qualityDropdown.AddOptions(options);
        
        int currentQualityIndex = QualitySettings.GetQualityLevel();
        qualityDropdown.value = PlayerPrefs.GetInt("QualitySettingPreference", currentQualityIndex);
        qualityDropdown.RefreshShownValue();
        qualityDropdown.onValueChanged.AddListener(SetQuality);
    }

    private void SetupFullscreenText()
    {
        fullscreenButtonText.text = Screen.fullScreen ? "Windowed" : "Fullscreen";
        fullscreenButton.onClick.AddListener(ToggleFullscreenMode);
    }

    private void SetupMouseSensitivitySlider()
    {
        mouseSensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 50f);
        mouseSensitivitySlider.onValueChanged.AddListener(SetMouseSensitivity);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = _resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionPreference", resolutionIndex);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex, true);
        PlayerPrefs.SetInt("QualitySettingPreference", qualityIndex);
    }

    private void SetMouseSensitivity(float sensitivity)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity);
    }

    public void ToggleFullscreenMode()
    {
        Screen.fullScreen = !Screen.fullScreen;
        UpdateFullscreenButtonText();
        PlayerPrefs.SetInt("FullscreenPreference", System.Convert.ToInt32(Screen.fullScreen));
    }

    private void UpdateFullscreenButtonText()
    {
        fullscreenButtonText.text = Screen.fullScreen ? "Windowed" : "Fullscreen";
    }

    public void ExitSettings()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void SetDefaultSettings()
    {
        int highestQualityIndex = QualitySettings.names.Length - 1;
        qualityDropdown.value = highestQualityIndex;
        SetQuality(highestQualityIndex);

        mouseSensitivitySlider.value = 100f;
        SetMouseSensitivity(100f);

        int highestResolutionIndex = _resolutions.Length - 1;
        resolutionDropdown.value = highestResolutionIndex;
        SetResolution(highestResolutionIndex);

        Screen.fullScreen = true;
        UpdateFullscreenButtonText();
    }
}