using UnityEngine;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;
    public Music_Manager musicManager;

    void Start()
    {
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);

        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;

        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    private void SetMusicVolume(float volume)
    {
        musicManager.audioSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    private void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();

        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.SetSFXVolume(volume);
        }
    }
}