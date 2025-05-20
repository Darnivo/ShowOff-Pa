using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public Slider volumeSlider;
    public Toggle fullscreenToggle;
    // public TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;

    void Start()
    {
        // Volume
        volumeSlider.value = PlayerPrefs.GetFloat("volume", 0.5f);
        AudioListener.volume = volumeSlider.value;
        volumeSlider.onValueChanged.AddListener(SetVolume);

        // Fullscreen
        bool isFullscreen = PlayerPrefs.GetInt("fullscreen", 1) == 1;
        fullscreenToggle.isOn = isFullscreen;
        Screen.fullScreen = isFullscreen;
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);

        // // Resolution
        // resolutions = Screen.resolutions;
        // resolutionDropdown.ClearOptions();
        // int currentIndex = 0;
        // var options = new System.Collections.Generic.List<string>();

        // for (int i = 0; i < resolutions.Length; i++)
        // {
        //     string option = resolutions[i].width + " x " + resolutions[i].height;
        //     options.Add(option);

        //     if (resolutions[i].width == Screen.currentResolution.width &&
        //         resolutions[i].height == Screen.currentResolution.height)
        //     {
        //         currentIndex = i;
        //     }
        // }

        // resolutionDropdown.AddOptions(options);
        // resolutionDropdown.value = PlayerPrefs.GetInt("resolutionIndex", currentIndex);
        // resolutionDropdown.onValueChanged.AddListener(SetResolution);
        // resolutionDropdown.RefreshShownValue();

        // SetResolution(resolutionDropdown.value);
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("volume", value);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
    }

    // public void SetResolution(int index)
    // {
    //     Resolution res = resolutions[index];
    //     Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    //     PlayerPrefs.SetInt("resolutionIndex", index);
    // }






}
