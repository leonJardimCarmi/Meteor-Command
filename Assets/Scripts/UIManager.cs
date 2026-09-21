using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI _scoreLabel;
    [SerializeField] private TextMeshProUGUI _ammoLabel;
    [SerializeField] private TextMeshProUGUI _waveLabel;

    [Header("Main Menu")]
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private Button _playButton;

    [Header("Game Over")]
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private TextMeshProUGUI _finalScoreLabel;
    [SerializeField] private Button _restartButton;
    [SerializeField] private float _restartLockout = 0.5f;

    private void OnEnable()
    {
        GameManager.ScoreChanged += ShowScore;
        GameManager.GameStarted += HideMenu;
        GameManager.GameOver += ShowGameOver;
        Battery.AmmoChanged += ShowAmmo;
        WaveSpawner.WaveStarted += ShowWave;
        _playButton.onClick.AddListener(StartGame);
        _restartButton.onClick.AddListener(Restart);
        ShowScore(0);
    }

    private void OnDisable()
    {
        GameManager.ScoreChanged -= ShowScore;
        GameManager.GameStarted -= HideMenu;
        GameManager.GameOver -= ShowGameOver;
        Battery.AmmoChanged -= ShowAmmo;
        WaveSpawner.WaveStarted -= ShowWave;
        _playButton.onClick.RemoveListener(StartGame);
        _restartButton.onClick.RemoveListener(Restart);
    }

    private void Start()
    {
        _menuPanel.SetActive(!GameManager.Instance.IsPlaying);
    }

    private void ShowScore(int score)
    {
        _scoreLabel.text = $"Score: {score}";
    }

    private void ShowAmmo(int ammo)
    {
        _ammoLabel.text = $"Ammo: {ammo}";
    }

    private void ShowWave(int wave)
    {
        _waveLabel.text = $"Wave {wave}";
    }

    private void HideMenu()
    {
        _menuPanel.SetActive(false);
    }

    private void ShowGameOver()
    {
        _finalScoreLabel.text = $"Final score: {GameManager.Instance.Score}";
        _gameOverPanel.SetActive(true);
        StartCoroutine(UnlockRestart());
    }

    private IEnumerator UnlockRestart()
    {
        _restartButton.interactable = false;
        yield return new WaitForSeconds(_restartLockout);
        _restartButton.interactable = true;
    }

    private void StartGame()
    {
        GameManager.Instance.StartGame();
    }

    private void Restart()
    {
        GameManager.Instance.Restart();
    }
}
