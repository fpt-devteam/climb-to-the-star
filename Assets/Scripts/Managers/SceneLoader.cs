using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles asynchronous scene loading with loading screens and progress tracking
/// </summary>
public class SceneLoader : MonoBehaviour
{
    [Header("Loading Screen")]
    [SerializeField]
    private GameObject _loadingScreenPrefab;

    [SerializeField]
    private Canvas _loadingCanvas;

    [SerializeField]
    private UnityEngine.UI.Slider _progressBar;

    [SerializeField]
    private TMPro.TextMeshProUGUI _progressText;

    [Header("Scene Settings")]
    [SerializeField]
    private float _minimumLoadTime = 1f;

    [SerializeField]
    private bool _showLoadingScreen = true;

    // Singleton pattern
    public static SceneLoader Instance { get; private set; }

    // Events
    public System.Action<float> OnLoadProgressChanged;
    public System.Action<string> OnSceneLoadStarted;
    public System.Action<string> OnSceneLoadCompleted;

    // Private fields
    private GameObject _currentLoadingScreen;
    private bool _isLoading = false;
    private string _currentSceneName;
    private AsyncOperation _currentLoadOperation;

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
        OnLoadProgressChanged = null;
        OnSceneLoadStarted = null;
        OnSceneLoadCompleted = null;
    }

    #endregion

    #region Initialization

    public void Initialize()
    {
        // Subscribe to scene load events
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _isLoading = false;
        _currentSceneName = scene.name;
        OnSceneLoadCompleted?.Invoke(scene.name);
    }

    private void OnSceneUnloaded(Scene scene)
    {
        // Cleanup when scene is unloaded
    }

    #endregion

    #region Scene Loading

    public void LoadScene(string sceneName)
    {
        if (_isLoading)
            return;

        StartCoroutine(LoadSceneAsync(sceneName));
    }

    public void LoadScene(int sceneIndex)
    {
        if (_isLoading)
            return;

        string sceneName = SceneUtility.GetScenePathByBuildIndex(sceneIndex);
        sceneName = System.IO.Path.GetFileNameWithoutExtension(sceneName);
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    public void LoadSceneAdditive(string sceneName)
    {
        if (_isLoading)
            return;

        StartCoroutine(LoadSceneAdditiveAsync(sceneName));
    }

    public void UnloadScene(string sceneName)
    {
        StartCoroutine(UnloadSceneAsync(sceneName));
    }

    public void ReloadCurrentScene()
    {
        if (_isLoading)
            return;

        string currentScene = SceneManager.GetActiveScene().name;
        StartCoroutine(LoadSceneAsync(currentScene));
    }

    #endregion

    #region Async Loading Coroutines

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        _isLoading = true;
        _currentSceneName = sceneName;
        OnSceneLoadStarted?.Invoke(sceneName);

        // Show loading screen
        if (_showLoadingScreen)
        {
            ShowLoadingScreen();
        }

        // Start loading the scene
        _currentLoadOperation = SceneManager.LoadSceneAsync(sceneName);
        _currentLoadOperation.allowSceneActivation = false;

        float startTime = Time.time;
        float progress = 0f;

        // Wait for scene to load
        while (_currentLoadOperation.progress < 0.9f)
        {
            progress = _currentLoadOperation.progress;
            UpdateLoadingProgress(progress);
            yield return null;
        }

        // Ensure minimum load time
        float elapsedTime = Time.time - startTime;
        if (elapsedTime < _minimumLoadTime)
        {
            yield return new WaitForSeconds(_minimumLoadTime - elapsedTime);
        }

        // Complete loading
        _currentLoadOperation.allowSceneActivation = true;
        progress = 1f;
        UpdateLoadingProgress(progress);

        // Wait for scene activation
        while (!_currentLoadOperation.isDone)
        {
            yield return null;
        }

        // Hide loading screen
        if (_showLoadingScreen)
        {
            HideLoadingScreen();
        }

        _isLoading = false;
        _currentLoadOperation = null;
    }

    private IEnumerator LoadSceneAdditiveAsync(string sceneName)
    {
        _isLoading = true;
        OnSceneLoadStarted?.Invoke(sceneName);

        // Show loading screen
        if (_showLoadingScreen)
        {
            ShowLoadingScreen();
        }

        // Start loading the scene additively
        _currentLoadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        _currentLoadOperation.allowSceneActivation = false;

        float startTime = Time.time;
        float progress = 0f;

        // Wait for scene to load
        while (_currentLoadOperation.progress < 0.9f)
        {
            progress = _currentLoadOperation.progress;
            UpdateLoadingProgress(progress);
            yield return null;
        }

        // Ensure minimum load time
        float elapsedTime = Time.time - startTime;
        if (elapsedTime < _minimumLoadTime)
        {
            yield return new WaitForSeconds(_minimumLoadTime - elapsedTime);
        }

        // Complete loading
        _currentLoadOperation.allowSceneActivation = true;
        progress = 1f;
        UpdateLoadingProgress(progress);

        // Wait for scene activation
        while (!_currentLoadOperation.isDone)
        {
            yield return null;
        }

        // Hide loading screen
        if (_showLoadingScreen)
        {
            HideLoadingScreen();
        }

        _isLoading = false;
        _currentLoadOperation = null;
    }

    private IEnumerator UnloadSceneAsync(string sceneName)
    {
        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(sceneName);

        while (!unloadOperation.isDone)
        {
            yield return null;
        }
    }

    #endregion

    #region Loading Screen Management

    private void ShowLoadingScreen()
    {
        if (_loadingScreenPrefab != null)
        {
            _currentLoadingScreen = Instantiate(_loadingScreenPrefab);
            DontDestroyOnLoad(_currentLoadingScreen);

            // Find UI components if not assigned
            if (_progressBar == null)
                _progressBar =
                    _currentLoadingScreen.GetComponentInChildren<UnityEngine.UI.Slider>();

            if (_progressText == null)
                _progressText =
                    _currentLoadingScreen.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        }
        else if (_loadingCanvas != null)
        {
            _loadingCanvas.gameObject.SetActive(true);
        }
    }

    private void HideLoadingScreen()
    {
        if (_currentLoadingScreen != null)
        {
            Destroy(_currentLoadingScreen);
            _currentLoadingScreen = null;
        }
        else if (_loadingCanvas != null)
        {
            _loadingCanvas.gameObject.SetActive(false);
        }
    }

    private void UpdateLoadingProgress(float progress)
    {
        OnLoadProgressChanged?.Invoke(progress);

        // Update UI elements
        if (_progressBar != null)
        {
            _progressBar.value = progress;
        }

        if (_progressText != null)
        {
            _progressText.text = $"Loading... {(progress * 100f):F0}%";
        }
    }

    #endregion

    #region Utility Methods

    public bool IsLoading => _isLoading;

    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    public int GetCurrentSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    public void SetMinimumLoadTime(float time)
    {
        _minimumLoadTime = Mathf.Max(0f, time);
    }

    public void SetShowLoadingScreen(bool show)
    {
        _showLoadingScreen = show;
    }

    public void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #endregion

    #region Scene Management

    public void LoadNextScene()
    {
        int currentIndex = GetCurrentSceneIndex();
        int nextIndex = (currentIndex + 1) % SceneManager.sceneCountInBuildSettings;
        LoadScene(nextIndex);
    }

    public void LoadPreviousScene()
    {
        int currentIndex = GetCurrentSceneIndex();
        int previousIndex =
            (currentIndex - 1 + SceneManager.sceneCountInBuildSettings)
            % SceneManager.sceneCountInBuildSettings;
        LoadScene(previousIndex);
    }

    public bool HasNextScene()
    {
        return GetCurrentSceneIndex() < SceneManager.sceneCountInBuildSettings - 1;
    }

    public bool HasPreviousScene()
    {
        return GetCurrentSceneIndex() > 0;
    }

    #endregion
}
