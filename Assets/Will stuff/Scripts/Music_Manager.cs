using UnityEngine;

public class Music_Manager : MonoBehaviour
{
    public AudioClip musicClip;

    private void Start()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = musicClip;
        audioSource.loop = true;
        audioSource.playOnAwake = true;
        audioSource.Play();
    }
}
