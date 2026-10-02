using System.Collections;
using TMPro;
using UnityEngine;

// A big word on the screen for a combo ("DOUBLE!", "TRIPLE!", "MEGA COMBO!"). It pops in, stays a moment and
// fades out. Every extra kill in the same blast replaces it with the next word, hotter in colour, so a growing
// combo is easy to follow. Timed in real seconds, so a slow-motion moment does not stretch it.
[RequireComponent(typeof(TextMeshProUGUI))]
public class ComboCallout : MonoBehaviour
{
    private static readonly string[] Words = { "DOUBLE!", "TRIPLE!", "MEGA COMBO!", "INSANE COMBO!" };

    private static readonly Color[] Colors =
    {
        new Color(1f, 0.95f, 0.6f),
        new Color(1f, 0.8f, 0.2f),
        new Color(1f, 0.5f, 0.15f),
        new Color(1f, 0.2f, 0.2f)
    };

    [SerializeField] private float _popScale = 1.6f;
    [SerializeField] private float _popSeconds = 0.15f;
    [SerializeField] private float _holdSeconds = 0.7f;
    [SerializeField] private float _fadeSeconds = 0.3f;

    private TextMeshProUGUI _label;
    private Coroutine _routine;

    private void Awake()
    {
        _label = GetComponent<TextMeshProUGUI>();
        _label.raycastTarget = false;
        _label.alpha = 0f;
    }

    private void OnEnable()
    {
        GameManager.KillScored += Show;
    }

    private void OnDisable()
    {
        GameManager.KillScored -= Show;
    }

    private void Show(Vector3 position, int points, int killNumber)
    {
        if (killNumber < 2)
        {
            return;
        }

        int level = Mathf.Min(killNumber - 2, Words.Length - 1);
        _label.text = Words[level];
        _label.color = Colors[level];

        if (_routine != null)
        {
            StopCoroutine(_routine);
        }

        _routine = StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        for (float progress = 0f; progress < 1f; progress += Time.unscaledDeltaTime / _popSeconds)
        {
            float ease = 1f - (1f - progress) * (1f - progress);
            transform.localScale = Vector3.one * Mathf.Lerp(_popScale, 1f, ease);
            yield return null;
        }

        transform.localScale = Vector3.one;
        yield return new WaitForSecondsRealtime(_holdSeconds);

        for (float progress = 0f; progress < 1f; progress += Time.unscaledDeltaTime / _fadeSeconds)
        {
            _label.alpha = 1f - progress;
            yield return null;
        }

        _label.alpha = 0f;
        _routine = null;
    }
}
