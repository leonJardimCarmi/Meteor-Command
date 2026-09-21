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
    [SerializeField] private int _lowAmmo = 5;
    [SerializeField] private Color _lowAmmoColor = new Color(1f, 0.25f, 0.2f);

    [Header("Main Menu")]
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private TextMeshProUGUI _menuBestLabel;

    [Header("Game Over")]
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private TextMeshProUGUI _finalScoreLabel;
    [SerializeField] private TextMeshProUGUI _gameOverBestLabel;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _menuButton;
    [SerializeField] private float _restartLockout = 0.5f;

    private Color _normalAmmoColor;

    private void Awake()
    {
        _normalAmmoColor = _ammoLabel.color;
    }

    private void OnEnable()
    {
        GameManager.ScoreChanged += ShowScore;
        GameManager.GameStarted += HideMenu;
        GameManager.GameOver += ShowGameOver;
        Battery.AmmoChanged += ShowAmmo;
        WaveSpawner.WaveStarted += ShowWave;
        _playButton.onClick.AddListener(StartGame);
        _quitButton.onClick.AddListener(Quit);
        _restartButton.onClick.AddListener(Restart);
        _menuButton.onClick.AddListener(ReturnToMenu);
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
        _quitButton.onClick.RemoveListener(Quit);
        _restartButton.onClick.RemoveListener(Restart);
        _menuButton.onClick.RemoveListener(ReturnToMenu);
    }

    private void Start()
    {
        _menuPanel.SetActive(!GameManager.Instance.IsPlaying);
        _menuBestLabel.text = $"Best: {GameManager.Instance.HighScore}";
    }

    private void ShowScore(int score)
    {
        _scoreLabel.text = $"Score: {score}";
    }

    private void ShowAmmo(int ammo)
    {
        _ammoLabel.text = $"Ammo: {ammo} / {GameManager.Instance.AmmoPerWave}";
        _ammoLabel.color = ammo < _lowAmmo ? _lowAmmoColor : _normalAmmoColor;
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
        _gameOverBestLabel.text = GameManager.Instance.IsNewHighScore ? "NEW BEST!" : $"Best: {GameManager.Instance.HighScore}";
        _gameOverPanel.SetActive(true);
        StartCoroutine(UnlockGameOverButtons());
    }

    private IEnumerator UnlockGameOverButtons()
    {
        SetGameOverButtonsInteractable(false);
        yield return new WaitForSeconds(_restartLockout);
        SetGameOverButtonsInteractable(true);
    }

    private void SetGameOverButtonsInteractable(bool interactable)
    {
        _restartButton.interactable = interactable;
        _menuButton.interactable = interactable;
    }

    private void StartGame()
    {
        GameManager.Instance.StartGame();
    }

    private void Quit()
    {
        GameManager.Instance.Quit();
    }

    private void Restart()
    {
        GameManager.Instance.Restart();
    }

    private void ReturnToMenu()
    {
        GameManager.Instance.ReturnToMenu();
    }
}
