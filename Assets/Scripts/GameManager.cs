using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static event Action GameOver;
    public static event Action<int> ScoreChanged;

    public static GameManager Instance { get; private set; }

    [SerializeField] private Battery _battery;
    [SerializeField] private int _killPoints = 100;
    [SerializeField] private int _ammoPerWave = 20;
    [SerializeField] private int _unusedAmmoBonus = 25;

    public bool IsGameOver { get; private set; }
    public int Score { get; private set; }

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
    }

    public void AddKill(int killNumber)
    {
        if (IsGameOver)
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
        IsGameOver = true;
        GameOver?.Invoke();
    }
}