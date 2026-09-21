using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Menu,
    Playing,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static event Action GameStarted;
    public static event Action GameOver;
    public static event Action<int> ScoreChanged;

    public static GameManager Instance { get; private set; }

    private static bool _skipMenu;

    [SerializeField] private Battery _battery;
    [SerializeField] private int _killPoints = 100;
    [SerializeField] private int _ammoPerWave = 20;
    [SerializeField] private int _unusedAmmoBonus = 25;

    public GameState State { get; private set; } = GameState.Menu;
    public int Score { get; private set; }

    public bool IsPlaying => State == GameState.Playing;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        City.Destroyed += OnCityDestroyed;
    }

    private void OnDisable()
    {
        City.Destroyed -= OnCityDestroyed;
    }

    private void Start()
    {
        _battery.Refill(_ammoPerWave);

        if (_skipMenu)
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        State = GameState.Playing;
        GameStarted?.Invoke();
    }

    public void Restart()
    {
        _skipMenu = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void AddKill(int killNumber)
    {
        if (!IsPlaying)
        {
            return;
        }

        // The Nth kill of one blast is worth (2N - 1) x base points, so N kills total N x N x base.
        AddScore(_killPoints * (2 * killNumber - 1));
    }

    public void CompleteWave()
    {
        AddScore(_battery.Ammo * _unusedAmmoBonus);
        _battery.Refill(_ammoPerWave);
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
        GameOver?.Invoke();
    }
}
