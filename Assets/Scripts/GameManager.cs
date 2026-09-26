using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Menu,
    Playing,
    Paused,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static event Action GameStarted;
    public static event Action GameOver;
    public static event Action GamePaused;
    public static event Action GameResumed;
    public static event Action<int> ScoreChanged;
    public static event Action<Vector3, int, int> KillScored;

    public static GameManager Instance { get; private set; }

    private const string HighScoreKey = "HighScore";

    private static bool _skipMenu;

    [SerializeField] private Battery _battery;
    [SerializeField] private int _killPoints = 100;
    [SerializeField] [Range(0f, 1f)] private float _extraAmmoFraction = 0.25f;
    [SerializeField] private int _unusedAmmoBonus = 25;

    public GameState State { get; private set; } = GameState.Menu;
    public int Score { get; private set; }
    public int HighScore { get; private set; }
    public bool IsNewHighScore { get; private set; }

    public bool IsPlaying => State == GameState.Playing;

    // How many shots the battery gets for the wave now being armed. Set from the wave's own meteor count
    // (see OnWaveSizeDetermined), so ammo keeps pace as later waves throw more meteors at once.
    public int AmmoPerWave { get; private set; }

    private void Awake()
    {
        Instance = this;
        HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        SetPaused(false);
    }

    private void OnEnable()
    {
        City.Destroyed += OnCityDestroyed;
        WaveSpawner.WaveSizeDetermined += OnWaveSizeDetermined;
    }

    private void OnDisable()
    {
        City.Destroyed -= OnCityDestroyed;
        WaveSpawner.WaveSizeDetermined -= OnWaveSizeDetermined;
    }

    private void Start()
    {
        if (_skipMenu)
        {
            StartGame();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void StartGame()
    {
        State = GameState.Playing;
        GameStarted?.Invoke();
    }

    public void Pause()
    {
        if (State != GameState.Playing)
        {
            return;
        }

        State = GameState.Paused;
        SetPaused(true);
        GamePaused?.Invoke();
    }

    public void Resume()
    {
        if (State != GameState.Paused)
        {
            return;
        }

        State = GameState.Playing;
        SetPaused(false);
        GameResumed?.Invoke();
    }

    public void Restart()
    {
        ReloadScene(skipMenu: true);
    }

    public void ReturnToMenu()
    {
        ReloadScene(skipMenu: false);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void AddKill(int killNumber, Vector3 position)
    {
        if (!IsPlaying)
        {
            return;
        }

        // The Nth kill of one blast is worth (2N - 1) x base points, so N kills total N x N x base.
        int points = _killPoints * (2 * killNumber - 1);
        AddScore(points);
        KillScored?.Invoke(position, points, killNumber);
    }

    public void CompleteWave()
    {
        AddScore(_battery.Ammo * _unusedAmmoBonus);
    }

    // A wave always brings at least enough shots to clear it, plus a working margin, so a later wave with
    // more meteors than the old fixed ammo count is never impossible to finish.
    private void OnWaveSizeDetermined(int meteorCount)
    {
        AmmoPerWave = Mathf.CeilToInt(meteorCount * (1f + _extraAmmoFraction));
        _battery.Refill(AmmoPerWave);
    }

    private void TogglePause()
    {
        if (State == GameState.Playing)
        {
            Pause();
        }
        else
        {
            Resume();
        }
    }

    // Freezes or unfreezes time and audio together, so a pause is really a pause.
    private void SetPaused(bool paused)
    {
        Time.timeScale = paused ? 0f : 1f;
        AudioListener.pause = paused;
    }

    private void ReloadScene(bool skipMenu)
    {
        _skipMenu = skipMenu;
        SetPaused(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void AddScore(int points)
    {
        Score += points;
        ScoreChanged?.Invoke(Score);
    }

    private void OnCityDestroyed()
    {
        if (CityManager.Instance.CountAlive() == 0)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        State = GameState.GameOver;
        SaveHighScore();
        GameOver?.Invoke();
    }

    private void SaveHighScore()
    {
        IsNewHighScore = Score > HighScore;

        if (IsNewHighScore)
        {
            HighScore = Score;
            PlayerPrefs.SetInt(HighScoreKey, HighScore);
            PlayerPrefs.Save();
        }
    }
}
