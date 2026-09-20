using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    private TextMeshProUGUI _label;

    private void Awake()
    {
        _label = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        GameManager.ScoreChanged += Show;
        Show(0);
    }

    private void OnDisable()
    {
        GameManager.ScoreChanged -= Show;
    }

    private void Show(int score)
    {
        _label.text = $"Score: {score}";
    }
}