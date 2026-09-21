using System.Collections;
using TMPro;
using UnityEngine;

// A big "WAVE N" banner in the middle of the screen at the start of every wave. It stays visible for
// the hold time, then fades out. Together they should match the pause before the meteors start.
[RequireComponent(typeof(TextMeshProUGUI))]
public class WaveBanner : MonoBehaviour
{
    [SerializeField] private float _holdSeconds = 1.5f;
    [SerializeField] private float _fadeSeconds = 0.5f;

    private TextMeshProUGUI _label;
    private Coroutine _routine;

    private void Awake()
    {
        _label = GetComponent<TextMeshProUGUI>();
        SetAlpha(0f);
    }

    private void OnEnable()
    {
        WaveSpawner.WaveStarted += Show;
    }

    private void OnDisable()
    {
        WaveSpawner.WaveStarted -= Show;
    }

    private void Show(int wave)
    {
        _label.text = $"WAVE {wave}";

        if (_routine != null)
        {
            StopCoroutine(_routine);
        }

        _routine = StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        SetAlpha(1f);
        yield return new WaitForSeconds(_holdSeconds);

        for (float progress = 0f; progress < 1f; progress += Time.deltaTime / _fadeSeconds)
        {
            SetAlpha(1f - progress);
            yield return null;
        }

        SetAlpha(0f);
        _routine = null;
    }

    private void SetAlpha(float alpha)
    {
        Color color = _label.color;
        color.a = alpha;
        _label.color = color;
    }
}
