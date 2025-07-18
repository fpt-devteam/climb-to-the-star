using System;
using System.Collections.Generic;
using UnityEngine;

public enum AudioSFXEnum
{
    ButtonClick,
    PlayerMove,
    PlayerLand,
    PlayerAttack,
    PlayerJump,
    PlayerHurt,
    EnemyAttack,
    EnemyHurt,
    BossAttack,
    BossHurt,
    BossDeath,
}

public enum AudioMusicEnum
{
    MainMenu,
    Level1,
    Level2,
    Level3,
    Level4,
    Level5,
}

[Serializable]
public struct SoundSFXData
{
    [SerializeField]
    public AudioSFXEnum key;

    [SerializeField]
    public AudioClip clip;
}

[Serializable]
public struct SoundMusicData
{
    [SerializeField]
    public AudioMusicEnum key;

    [SerializeField]
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Data")]
    [SerializeField]
    private SoundSFXData[] audioSFXClips;

    [SerializeField]
    private SoundMusicData[] audioMusicClips;

    [Header("Volume Settings")]
    [SerializeField]
    private float musicVolume = 1f;

    [SerializeField]
    private float sfxVolume = 1f;
    private Dictionary<AudioSFXEnum, SoundSFXData> audioSFXDictionary;
    private Dictionary<AudioMusicEnum, SoundMusicData> audioMusicDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioManager();
            SetMusicVolume(musicVolume);
            SetSFXVolume(sfxVolume);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioManager()
    {
        audioSFXDictionary = new Dictionary<AudioSFXEnum, SoundSFXData>();
        audioMusicDictionary = new Dictionary<AudioMusicEnum, SoundMusicData>();

        foreach (var soundData in audioSFXClips)
        {
            if (soundData.clip != null)
                audioSFXDictionary[soundData.key] = soundData;
        }

        foreach (var soundData in audioMusicClips)
        {
            if (soundData.clip != null)
                audioMusicDictionary[soundData.key] = soundData;
        }
    }

    public void PlayMusic(AudioMusicEnum key)
    {
        if (!audioMusicDictionary.ContainsKey(key))
        {
            Debug.LogWarning($"Music key '{key}' not found in AudioManager");
            return;
        }
        var soundData = audioMusicDictionary[key];
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
        musicSource.clip = soundData.clip;
        musicSource.Play();
    }

    public void PlaySFX(AudioSFXEnum key)
    {
        if (!audioSFXDictionary.ContainsKey(key))
        {
            Debug.LogWarning($"SFX key '{key}' not found in AudioManager");
            return;
        }
        var soundData = audioSFXDictionary[key];
        sfxSource.PlayOneShot(soundData.clip, sfxVolume);
    }

    public void PlaySFX(AudioSFXEnum key, float customVolume)
    {
        if (!audioSFXDictionary.ContainsKey(key))
        {
            Debug.LogWarning($"SFX key '{key}' not found in AudioManager");
            return;
        }
        var soundData = audioSFXDictionary[key];
        sfxSource.PlayOneShot(soundData.clip, customVolume);
    }

    public void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    public void StopSFX()
    {
        if (sfxSource != null && sfxSource.isPlaying)
        {
            sfxSource.Stop();
        }
    }

    public void PauseMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.UnPause();
        }
    }

    public void SetMusicVolume(float volume) => musicSource.volume = Mathf.Clamp01(volume);

    public void SetSFXVolume(float volume) => sfxSource.volume = Mathf.Clamp01(volume);

    public float GetMusicVolume() => musicSource.volume;

    public float GetSFXVolume() => sfxSource.volume;

    public void ToggleMusic() => musicSource.mute = !musicSource.mute;

    public void ToggleSFX() => sfxSource.mute = !sfxSource.mute;
}
