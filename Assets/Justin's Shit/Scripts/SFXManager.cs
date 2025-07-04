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
    public AudioClip dissolveInSound;
    public AudioClip dissolveOutSound;
    public AudioClip frogJumpSound;

    [Header("Audio Sources")]
    public AudioSource oneShotSource;
    public AudioSource walkingSource;
    public AudioSource fanSource;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float masterSFXVolume = 1f;

    [Header("Individual Volume Multipliers")]
    [Range(0f, 5f)]
    public float jumpVolumeMultiplier = 2f;
    [Range(0f, 5f)]
    public float ropeAttachVolumeMultiplier = 1f;
    [Range(0f, 5f)]
    public float trampolineVolumeMultiplier = 1.5f;
    [Range(0f, 5f)]
    public float walkingVolumeMultiplier = 0.8f;
    [Range(0f, 5f)]
    public float fanVolumeMultiplier = 0.5f;
    [Range(0f, 5f)]
    public float dissolveVolumeMultiplier = 0.7f;
    [Range(0f, 5f)]
    public float frogJumpVolumeMultiplier = 0.8f;

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
        UpdateAllVolumes();
    }

    void UpdateAllVolumes()
    {
        if (oneShotSource != null)
            oneShotSource.volume = masterSFXVolume;
        if (walkingSource != null)
            walkingSource.volume = masterSFXVolume * walkingVolumeMultiplier;
        if (fanSource != null)
            fanSource.volume = masterSFXVolume * fanVolumeMultiplier;
    }

    public void PlayJumpSound()
    {
        if (jumpSound != null && oneShotSource != null)
        {
            oneShotSource.PlayOneShot(jumpSound, masterSFXVolume * jumpVolumeMultiplier);
        }
    }

    public void PlayRopeAttachSound()
    {
        if (ropeAttachSound != null && oneShotSource != null)
        {
            oneShotSource.PlayOneShot(ropeAttachSound, masterSFXVolume * ropeAttachVolumeMultiplier);
        }
    }

    public void PlayTrampolineSound()
    {
        if (trampolineJumpSounds != null && trampolineJumpSounds.Length > 0 && oneShotSource != null)
        {
            int randomIndex = Random.Range(0, trampolineJumpSounds.Length);
            oneShotSource.PlayOneShot(trampolineJumpSounds[randomIndex], masterSFXVolume * trampolineVolumeMultiplier);
        }
    }

    public void PlayDissolveInSound()
    {
        if (dissolveInSound != null && oneShotSource != null)
        {
            oneShotSource.PlayOneShot(dissolveInSound, masterSFXVolume * dissolveVolumeMultiplier);
        }
    }

    public void PlayDissolveOutSound()
    {
        if (dissolveOutSound != null && oneShotSource != null)
        {
            oneShotSource.PlayOneShot(dissolveOutSound, masterSFXVolume * dissolveVolumeMultiplier);
        }
    }

    public void PlayFrogJumpSound()
    {
        if (frogJumpSound != null && oneShotSource != null)
        {
            oneShotSource.PlayOneShot(frogJumpSound, masterSFXVolume * frogJumpVolumeMultiplier);
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

    void OnValidate()
    {
        if (Application.isPlaying)
        {
            UpdateAllVolumes();
        }
    }
}