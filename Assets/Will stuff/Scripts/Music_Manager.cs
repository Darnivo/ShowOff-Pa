using UnityEngine;

public class Music_Manager : MonoBehaviour
{
    public AudioClip musicClip;
    [HideInInspector] public AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = musicClip;
        audioSource.loop = true;
        audioSource.playOnAwake = true;
    }

    private void Start()
    {
        audioSource.volume = PlayerPrefs.GetFloat("MusicVolume", 0.7f); 
        audioSource.Play();
    }
}
