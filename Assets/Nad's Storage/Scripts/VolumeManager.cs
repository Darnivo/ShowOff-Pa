using UnityEngine;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;
    public Music_Manager musicManager;

    void Start()
    {
        float musicVolume = 0.5f;
        float sfxVolume = 0.5f; 
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    private void SetMusicVolume(float volume)
    {
        // Debug.Log("Setting music volume to: " + volume);
        musicManager.audioSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
        // PlayerPrefs.Save();

    }

    private void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFXVolume", volume);

        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.SetSFXVolume(volume);
        }
    }
}