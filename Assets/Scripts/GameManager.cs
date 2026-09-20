using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static event Action GameOver;

    public static GameManager Instance { get; private set; }

    public bool IsGameOver { get; private set; }

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

    private void EndGame()
    {
        IsGameOver = true;
        GameOver?.Invoke();
        Debug.Log("Game Over");
    }
}