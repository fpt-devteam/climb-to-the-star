using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver,
    Victory,
}

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public static GameManager Instance { get; private set; }
    public GameState CurrentGameState => currentGameState;
    public bool IsPaused => isPaused;
    private bool isPaused = false;
    private GameState currentGameState;

    private void Awake()
    {
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

    private void Start()
    {
        ChangeGameState(GameState.MainMenu);
    }

    public void ChangeGameState(GameState newState)
    {
        if (currentGameState == newState)
            return;

        currentGameState = newState;

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
                RestartGame();
                break;
            case GameState.Victory:
                HandleVictoryState();
                break;
        }
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

    private void HandleVictoryState()
    {
        SetPauseState(true);
        Time.timeScale = 0f;
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (currentGameState == GameState.Playing)
        {
            ChangeGameState(GameState.Paused);
        }
        else if (currentGameState == GameState.Paused)
        {
            ChangeGameState(GameState.Playing);
        }
    }

    private void SetPauseState(bool isPaused) => this.isPaused = isPaused;

    public void GameOver() => ChangeGameState(GameState.GameOver);

    public void StartGame() => ChangeGameState(GameState.Playing);

    public void Victory() => ChangeGameState(GameState.Victory);

    public void RestartGame()
    {
        SceneLoader.Instance.ReloadCurrentScene();
        ChangeGameState(GameState.Playing);
    }

    public void QuitGame() => Application.Quit();
}
