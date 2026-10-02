using System.Collections;
using UnityEngine;

// A short slow-motion moment for the big events: a big combo, a city being lost, or the turret being destroyed. The length is timed in real
// seconds, so it lasts the same however slow the game itself is running.
public class SlowMotion : MonoBehaviour
{
    [Header("Big combo")]
    [SerializeField] private int _comboKills = 3;
    [SerializeField] private float _comboSeconds = 0.6f;
    [SerializeField] [Range(0.05f, 1f)] private float _comboSpeed = 0.3f;

    [Header("City lost")]
    [SerializeField] private float _cityLostSeconds = 0.8f;
    [SerializeField] [Range(0.05f, 1f)] private float _cityLostSpeed = 0.25f;

    private Coroutine _routine;

    private void OnEnable()
    {
        GameManager.KillScored += OnKillScored;
        City.Destroyed += OnBigLoss;
        TurretHealth.Destroyed += OnBigLoss;
    }

    private void OnDisable()
    {
        GameManager.KillScored -= OnKillScored;
        City.Destroyed -= OnBigLoss;
        TurretHealth.Destroyed -= OnBigLoss;
    }

    private void OnKillScored(Vector3 position, int points, int killNumber)
    {
        if (killNumber == _comboKills)
        {
            Play(_comboSeconds, _comboSpeed);
        }
    }

    private void OnBigLoss()
    {
        Play(_cityLostSeconds, _cityLostSpeed);
    }

    private void Play(float seconds, float speed)
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
        }

        _routine = StartCoroutine(SlowDown(seconds, speed));
    }

    private IEnumerator SlowDown(float seconds, float speed)
    {
        Time.timeScale = speed;
        yield return new WaitForSecondsRealtime(seconds);

        // A pause that started during the slow motion keeps the time frozen until the game is resumed.
        if (GameManager.Instance.State != GameState.Paused)
        {
            Time.timeScale = 1f;
        }

        _routine = null;
    }
}
