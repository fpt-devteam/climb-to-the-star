using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Collections;

namespace Managers
{
    /// <summary>
    /// Manages all audio playback with object pooling for performance
    /// </summary>
    public class SoundManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private AudioSource _uiSource;

        [Header("Audio Mixer")]
        [SerializeField] private AudioMixer _audioMixer;

        [Header("Audio Settings")]
        [SerializeField, Range(0f, 1f)] private float _masterVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float _musicVolume = 0.8f;
        [SerializeField, Range(0f, 1f)] private float _sfxVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float _uiVolume = 1f;

        [Header("Pool Settings")]
        [SerializeField] private int _audioSourcePoolSize = 10;
        [SerializeField] private GameObject _audioSourcePrefab;

        // Singleton pattern
        public static SoundManager Instance { get; private set; }

        // Events
        public System.Action<float> OnMasterVolumeChanged;
        public System.Action<float> OnMusicVolumeChanged;
        public System.Action<float> OnSFXVolumeChanged;
        public System.Action<float> OnUIVolumeChanged;

        // Properties
        public float MasterVolume => _masterVolume;
        public float MusicVolume => _musicVolume;
        public float SFXVolume => _sfxVolume;
        public float UIVolume => _uiVolume;

        // Private fields
        private Queue<AudioSource> _audioSourcePool;
        private List<AudioSource> _activeAudioSources;
        private bool _isInitialized = false;

        #region Unity Lifecycle

        private void Awake()
        {
            // Singleton pattern implementation
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            // Cleanup events
            OnMasterVolumeChanged = null;
            OnMusicVolumeChanged = null;
            OnSFXVolumeChanged = null;
            OnUIVolumeChanged = null;
        }

        #endregion

        #region Initialization

        public void Initialize()
        {
            if (_isInitialized) return;

            InitializeAudioSources();
            InitializeAudioSourcePool();
            LoadAudioSettings();
            _isInitialized = true;
        }

        private void InitializeAudioSources()
        {
            // Create audio sources if they don't exist
            if (_musicSource == null)
            {
                GameObject musicGO = new GameObject("MusicSource");
                musicGO.transform.SetParent(transform);
                _musicSource = musicGO.AddComponent<AudioSource>();
                _musicSource.loop = true;
                _musicSource.playOnAwake = false;
            }

            if (_sfxSource == null)
            {
                GameObject sfxGO = new GameObject("SFXSource");
                sfxGO.transform.SetParent(transform);
                _sfxSource = sfxGO.AddComponent<AudioSource>();
                _sfxSource.loop = false;
                _sfxSource.playOnAwake = false;
            }

            if (_uiSource == null)
            {
                GameObject uiGO = new GameObject("UISource");
                uiGO.transform.SetParent(transform);
                _uiSource = uiGO.AddComponent<AudioSource>();
                _uiSource.loop = false;
                _uiSource.playOnAwake = false;
            }
        }

        private void InitializeAudioSourcePool()
        {
            _audioSourcePool = new Queue<AudioSource>();
            _activeAudioSources = new List<AudioSource>();

            // Create pool of audio sources
            for (int i = 0; i < _audioSourcePoolSize; i++)
            {
                CreatePooledAudioSource();
            }
        }

        private void CreatePooledAudioSource()
        {
            GameObject audioGO = new GameObject($"PooledAudioSource_{_audioSourcePool.Count}");
            audioGO.transform.SetParent(transform);
            
            AudioSource audioSource = audioGO.AddComponent<AudioSource>();
            audioSource.loop = false;
            audioSource.playOnAwake = false;
            
            _audioSourcePool.Enqueue(audioSource);
        }

        private void LoadAudioSettings()
        {
            // Load saved audio settings from PlayerPrefs
            _masterVolume = PlayerPrefs.GetFloat("MasterVolume", _masterVolume);
            _musicVolume = PlayerPrefs.GetFloat("MusicVolume", _musicVolume);
            _sfxVolume = PlayerPrefs.GetFloat("SFXVolume", _sfxVolume);
            _uiVolume = PlayerPrefs.GetFloat("UIVolume", _uiVolume);

            ApplyAudioSettings();
        }

        #endregion

        #region Audio Playback

        public void PlayMusic(AudioClip musicClip, bool fadeIn = true)
        {
            if (musicClip == null) return;

            if (fadeIn)
            {
                StartCoroutine(FadeInMusic(musicClip));
            }
            else
            {
                _musicSource.clip = musicClip;
                _musicSource.Play();
            }
        }

        public void StopMusic(bool fadeOut = true)
        {
            if (fadeOut)
            {
                StartCoroutine(FadeOutMusic());
            }
            else
            {
                _musicSource.Stop();
            }
        }

        public void PlaySFX(AudioClip sfxClip, float volume = 1f, float pitch = 1f)
        {
            if (sfxClip == null) return;

            AudioSource audioSource = GetPooledAudioSource();
            if (audioSource != null)
            {
                audioSource.clip = sfxClip;
                audioSource.volume = volume * _sfxVolume * _masterVolume;
                audioSource.pitch = pitch;
                audioSource.Play();

                StartCoroutine(ReturnToPool(audioSource, sfxClip.length));
            }
        }

        public void PlayUI(AudioClip uiClip, float volume = 1f)
        {
            if (uiClip == null) return;

            _uiSource.clip = uiClip;
            _uiSource.volume = volume * _uiVolume * _masterVolume;
            _uiSource.Play();
        }

        #endregion

        #region Audio Source Pooling

        private AudioSource GetPooledAudioSource()
        {
            if (_audioSourcePool.Count > 0)
            {
                AudioSource audioSource = _audioSourcePool.Dequeue();
                _activeAudioSources.Add(audioSource);
                return audioSource;
            }

            // Create new audio source if pool is empty
            CreatePooledAudioSource();
            AudioSource newSource = _audioSourcePool.Dequeue();
            _activeAudioSources.Add(newSource);
            return newSource;
        }

        private IEnumerator ReturnToPool(AudioSource audioSource, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (audioSource != null)
            {
                audioSource.Stop();
                audioSource.clip = null;
                _activeAudioSources.Remove(audioSource);
                _audioSourcePool.Enqueue(audioSource);
            }
        }

        #endregion

        #region Volume Control

        public void SetMasterVolume(float volume)
        {
            _masterVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat("MasterVolume", _masterVolume);
            ApplyAudioSettings();
            OnMasterVolumeChanged?.Invoke(_masterVolume);
        }

        public void SetMusicVolume(float volume)
        {
            _musicVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat("MusicVolume", _musicVolume);
            ApplyAudioSettings();
            OnMusicVolumeChanged?.Invoke(_musicVolume);
        }

        public void SetSFXVolume(float volume)
        {
            _sfxVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat("SFXVolume", _sfxVolume);
            ApplyAudioSettings();
            OnSFXVolumeChanged?.Invoke(_sfxVolume);
        }

        public void SetUIVolume(float volume)
        {
            _uiVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat("UIVolume", _uiVolume);
            ApplyAudioSettings();
            OnUIVolumeChanged?.Invoke(_uiVolume);
        }

        private void ApplyAudioSettings()
        {
            // Apply to audio sources
            if (_musicSource != null)
                _musicSource.volume = _musicVolume * _masterVolume;
            
            if (_sfxSource != null)
                _sfxSource.volume = _sfxVolume * _masterVolume;
            
            if (_uiSource != null)
                _uiSource.volume = _uiVolume * _masterVolume;

            // Apply to audio mixer if available
            if (_audioMixer != null)
            {
                _audioMixer.SetFloat("MasterVolume", Mathf.Log10(_masterVolume) * 20f);
                _audioMixer.SetFloat("MusicVolume", Mathf.Log10(_musicVolume) * 20f);
                _audioMixer.SetFloat("SFXVolume", Mathf.Log10(_sfxVolume) * 20f);
                _audioMixer.SetFloat("UIVolume", Mathf.Log10(_uiVolume) * 20f);
            }
        }

        #endregion

        #region Fade Coroutines

        private IEnumerator FadeInMusic(AudioClip musicClip)
        {
            _musicSource.clip = musicClip;
            _musicSource.volume = 0f;
            _musicSource.Play();

            float targetVolume = _musicVolume * _masterVolume;
            float fadeTime = 1f;
            float elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.deltaTime;
                _musicSource.volume = Mathf.Lerp(0f, targetVolume, elapsedTime / fadeTime);
                yield return null;
            }

            _musicSource.volume = targetVolume;
        }

        private IEnumerator FadeOutMusic()
        {
            float startVolume = _musicSource.volume;
            float fadeTime = 1f;
            float elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.deltaTime;
                _musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / fadeTime);
                yield return null;
            }

            _musicSource.Stop();
            _musicSource.volume = startVolume;
        }

        #endregion

        #region Utility Methods

        public void PauseAllAudio()
        {
            _musicSource?.Pause();
            _sfxSource?.Pause();
            _uiSource?.Pause();

            foreach (AudioSource source in _activeAudioSources)
            {
                if (source != null)
                    source.Pause();
            }
        }

        public void ResumeAllAudio()
        {
            _musicSource?.UnPause();
            _sfxSource?.UnPause();
            _uiSource?.UnPause();

            foreach (AudioSource source in _activeAudioSources)
            {
                if (source != null)
                    source.UnPause();
            }
        }

        public void StopAllAudio()
        {
            _musicSource?.Stop();
            _sfxSource?.Stop();
            _uiSource?.Stop();

            foreach (AudioSource source in _activeAudioSources)
            {
                if (source != null)
                    source.Stop();
            }
        }

        #endregion
    }
} 