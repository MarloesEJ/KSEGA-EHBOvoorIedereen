using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Boot,
    Playing,
    Paused,
    Win,
    Lose
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static void LoadNextLevelStatic()
    {
        if (Instance != null)
        {
            Instance.LoadNextLevel();
        }
    }

    [Header("Progress")]
    [SerializeField] private string[] levelSceneOrder;

    [Header("Runtime Data")]
    [SerializeField] private int currentLevelIndex;
    [SerializeField] private int score;
    [SerializeField] private int lives = 3;

    public GameState State { get; private set; } = GameState.Boot;
    public int Score => score;
    public int Lives => lives;
    public int CurrentLevelIndex => currentLevelIndex;

    public event Action<GameState> StateChanged;
    public event Action<int> LevelChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        State = GameState.Playing;
        StateChanged?.Invoke(State);
    }

    public void SetState(GameState newState)
    {
        if (State == newState)
        {
            return;
        }

        State = newState;
        StateChanged?.Invoke(State);
    }

    public void AddScore(int amount)
    {
        score += amount;
    }

    public void LoseLife(int amount = 1)
    {
        lives = Mathf.Max(0, lives - amount);
        if (lives == 0)
        {
            SetState(GameState.Lose);
        }
    }

    public void ResetRun()
    {
        score = 0;
        lives = 3;
        currentLevelIndex = 0;
        SetState(GameState.Playing);
    }

    public void LoadLevelByIndex(int index)
    {
        if (levelSceneOrder != null && levelSceneOrder.Length > 0)
        {
            if (index < 0 || index >= levelSceneOrder.Length)
            {
                return;
            }

            currentLevelIndex = index;
            SceneManager.LoadScene(levelSceneOrder[currentLevelIndex]);
        }
        else
        {
            if (index < 0 || index >= SceneManager.sceneCountInBuildSettings)
            {
                return;
            }

            currentLevelIndex = index;
            SceneManager.LoadScene(currentLevelIndex);
        }

        LevelChanged?.Invoke(currentLevelIndex);
    }

    public void LoadNextLevel()
    {
        LoadLevelByIndex(currentLevelIndex + 1);
    }

    public void ReloadCurrentLevel()
    {
        LoadLevelByIndex(currentLevelIndex);
    }
}
