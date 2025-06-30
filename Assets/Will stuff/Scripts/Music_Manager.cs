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
        // audioSource.volume = PlayerPrefs.GetFloat("MusicVolume", 1f); 
    }

    private void Start()
    {
        audioSource.Play();
    }
}
