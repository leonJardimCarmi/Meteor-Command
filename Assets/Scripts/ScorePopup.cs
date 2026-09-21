using System.Collections;
using TMPro;
using UnityEngine;

// A "+300" that floats up from where a meteor was destroyed and fades out. Bigger and warmer in colour
// for every extra kill in the same blast, so a combo is easy to notice. Pooled.
[RequireComponent(typeof(TextMeshPro))]
public class ScorePopup : MonoBehaviour
{
    [SerializeField] private float _baseFontSize = 10f;
    [SerializeField] private float _sizePerExtraKill = 0.3f;
    [SerializeField] private float _riseDistance = 2.5f;
    [SerializeField] private float _lifetime = 0.9f;
    [SerializeField] private Color _singleKillColor = Color.white;
    [SerializeField] private Color _comboColor = new Color(1f, 0.7f, 0.2f);
    [SerializeField] private int _killsForFullColor = 4;

    private TextMeshPro _label;

    private void Awake()
    {
        _label = GetComponent<TextMeshPro>();
    }

    public void Show(int points, int killNumber)
    {
        int extraKills = killNumber - 1;

        _label.text = $"+{points}";
        _label.fontSize = _baseFontSize * (1f + _sizePerExtraKill * extraKills);
        _label.color = Color.Lerp(_singleKillColor, _comboColor, (float)extraKills / (_killsForFullColor - 1));

        StartCoroutine(FloatUp());
    }

    private IEnumerator FloatUp()
    {
        Vector3 start = transform.position;
        Color color = _label.color;

        for (float progress = 0f; progress < 1f; progress += Time.deltaTime / _lifetime)
        {
            transform.position = start + Vector3.up * (_riseDistance * progress);
            color.a = 1f - progress * progress;
            _label.color = color;
            yield return null;
        }

        PoolManager.Instance.Release(gameObject);
    }
}
