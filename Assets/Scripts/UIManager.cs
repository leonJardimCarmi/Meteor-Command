using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreLabel;
    [SerializeField] private TextMeshProUGUI _ammoLabel;
    [SerializeField] private TextMeshProUGUI _waveLabel;

    private void OnEnable()
    {
        GameManager.ScoreChanged += ShowScore;
        Battery.AmmoChanged += ShowAmmo;
        WaveSpawner.WaveStarted += ShowWave;
        ShowScore(0);
    }

    private void OnDisable()
    {
        GameManager.ScoreChanged -= ShowScore;
        Battery.AmmoChanged -= ShowAmmo;
        WaveSpawner.WaveStarted -= ShowWave;
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
}