using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static event Action GameOver;
    public static event Action<int> ScoreChanged;
    [SerializeField] private int _killPoints = 100;

    public static GameManager Instance { get; private set; }

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

    private void OnCityDestroyed()
    {
        if (CityManager.Instance.CountAlive() == 0)
        {
            EndGame();
        }
    }
    public void AddKill(int killNumber)
    {
        if (IsGameOver)
        {
            return;
        }

        // The Nth kill of one blast is worth (2N - 1) points units, so N kills total N * N.
        Score += _killPoints * (2 * killNumber - 1);
        ScoreChanged?.Invoke(Score);
        Debug.Log($"Score: {Score}");
    }

    private void EndGame()
    {
        IsGameOver = true;
        GameOver?.Invoke();
    }
}