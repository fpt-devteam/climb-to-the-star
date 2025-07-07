using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace Managers
{
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver,
        Victory
    }

    public class GameManager : MonoBehaviour
    {
        [Header("Game Settings")]
        [SerializeField] private bool _isPaused = false;
        [SerializeField] private GameState _currentGameState = GameState.MainMenu;

        public static GameManager Instance { get; private set; }

        public System.Action<GameState> OnGameStateChanged;
        public System.Action<bool> OnPauseStateChanged;

        public bool IsPaused => _isPaused;
        public GameState CurrentGameState => _currentGameState;

        #region Unity Lifecycle

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeManagers();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            ChangeGameState(GameState.MainMenu);
        }

        private void OnDestroy()
        {
            OnGameStateChanged = null;
            OnPauseStateChanged = null;
        }

        #endregion

        #region Initialization

        private void InitializeManagers()
        {

        }

        #endregion

        #region Game State Management

        public void ChangeGameState(GameState newState)
        {
            if (_currentGameState == newState) return;

            GameState previousState = _currentGameState;
            _currentGameState = newState;

            switch (newState)
            {
                case GameState.MainMenu:
                    HandleMainMenuState();
                    break;
                case GameState.Playing:
                    HandlePlayingState();
                    break;
                case GameState.Paused:
                    HandlePausedState();
                    break;
                case GameState.GameOver:
                    HandleGameOverState();
                    break;
                case GameState.Victory:
                    HandleVictoryState();
                    break;
            }

            OnGameStateChanged?.Invoke(newState);
        }

        private void HandleMainMenuState()
        {
            SetPauseState(false);
            Time.timeScale = 1f;
        }

        private void HandlePlayingState()
        {
            SetPauseState(false);
            Time.timeScale = 1f;
        }

        private void HandlePausedState()
        {
            SetPauseState(true);
            Time.timeScale = 0f;
        }

        private void HandleGameOverState()
        {
            SetPauseState(true);
            Time.timeScale = 0f;
        }

        private void HandleVictoryState()
        {
            SetPauseState(true);
            Time.timeScale = 0f;
        }

        #endregion

        #region Pause Management

        public void TogglePause()
        {
            if (_currentGameState == GameState.Playing)
            {
                ChangeGameState(GameState.Paused);
            }
            else if (_currentGameState == GameState.Paused)
            {
                ChangeGameState(GameState.Playing);
            }
        }

        private void SetPauseState(bool isPaused)
        {
            if (_isPaused != isPaused)
            {
                _isPaused = isPaused;
                OnPauseStateChanged?.Invoke(isPaused);
            }
        }

        #endregion

        #region Game Flow Methods

        public void StartGame()
        {
            ChangeGameState(GameState.Playing);
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        public void GameOver()
        {
            ChangeGameState(GameState.GameOver);
        }

        public void Victory()
        {
            ChangeGameState(GameState.Victory);
        }

        #endregion
    }
} 