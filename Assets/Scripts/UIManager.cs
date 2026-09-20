using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreLabel;
    [SerializeField] private TextMeshProUGUI _ammoLabel;

    private void OnEnable()
    {
        GameManager.ScoreChanged += ShowScore;
        Battery.AmmoChanged += ShowAmmo;
        ShowScore(0);
    }

    private void OnDisable()
    {
        GameManager.ScoreChanged -= ShowScore;
        Battery.AmmoChanged -= ShowAmmo;
    }

    private void ShowScore(int score)
    {
        _scoreLabel.text = $"Score: {score}";
    }

    private void ShowAmmo(int ammo)
    {
        _ammoLabel.text = $"Ammo: {ammo}";
    }
}