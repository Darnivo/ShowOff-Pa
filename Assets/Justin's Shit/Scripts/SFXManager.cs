using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [Header("Sound Effects")]
    public AudioClip jumpSound;
    public AudioClip ropeAttachSound;
    public AudioClip[] trampolineJumpSounds;
    public AudioClip walkingSound;
    public AudioClip fanIdleSound;

    [Header("Audio Sources")]
    public AudioSource oneShotSource;
    public AudioSource walkingSource;
    public AudioSource fanSource;

    [Header("Settings")]
    [Range(0f, 1f)]
    public float masterSFXVolume = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadSFXVolume();
        StartFanSound();
    }

    void SetupAudioSources()
    {
        if (oneShotSource == null)
        {
            oneShotSource = gameObject.AddComponent<AudioSource>();
            oneShotSource.playOnAwake = false;
        }

        if (walkingSource == null)
        {
            walkingSource = gameObject.AddComponent<AudioSource>();
            walkingSource.playOnAwake = false;
            walkingSource.loop = true;
            walkingSource.clip = walkingSound;
        }

        if (fanSource == null)
        {
            fanSource = gameObject.AddComponent<AudioSource>();
            fanSource.playOnAwake = false;
            fanSource.loop = true;
            fanSource.clip = fanIdleSound;
        }
    }

    void LoadSFXVolume()
    {
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        SetSFXVolume(sfxVolume);
    }

    public void SetSFXVolume(float volume)
    {
        masterSFXVolume = volume;

        if (oneShotSource != null)
            oneShotSource.volume = masterSFXVolume;
        if (walkingSource != null)
            walkingSource.volume = masterSFXVolume * 0.3f;
        if (fanSource != null)
            fanSource.volume = masterSFXVolume * 0.2f;
    }

    public void PlayJumpSound()
    {
        if (jumpSound != null && oneShotSource != null)
        {
            oneShotSource.PlayOneShot(jumpSound, masterSFXVolume);
        }
    }

    public void PlayRopeAttachSound()
    {
        if (ropeAttachSound != null && oneShotSource != null)
        {
            oneShotSource.PlayOneShot(ropeAttachSound, masterSFXVolume);
        }
    }

    public void PlayTrampolineSound()
    {
        if (trampolineJumpSounds != null && trampolineJumpSounds.Length > 0 && oneShotSource != null)
        {
            int randomIndex = Random.Range(0, trampolineJumpSounds.Length);
            oneShotSource.PlayOneShot(trampolineJumpSounds[randomIndex], masterSFXVolume);
        }
    }

    public void StartWalkingSound()
    {
        if (walkingSource != null && !walkingSource.isPlaying)
        {
            walkingSource.Play();
        }
    }

    public void StopWalkingSound()
    {
        if (walkingSource != null && walkingSource.isPlaying)
        {
            walkingSource.Stop();
        }
    }

    void StartFanSound()
    {
        if (fanSource != null && fanIdleSound != null)
        {
            fanSource.Play();
        }
    }

    public void StopFanSound()
    {
        if (fanSource != null)
        {
            fanSource.Stop();
        }
    }

    public void PlayFanSound()
    {
        if (fanSource != null && !fanSource.isPlaying)
        {
            fanSource.Play();
        }
    }
}