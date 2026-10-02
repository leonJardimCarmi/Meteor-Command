using System.Collections;
using UnityEngine;

// Shakes the camera when a meteor hits the ground, or when a city is lost or the turret is hit (the same strong
// shake for both). The strongest shake wins,
// so overlapping hits do not add up or cancel each other.
public class CameraShake : MonoBehaviour
{
    [SerializeField] private float _duration = 0.35f;
    [SerializeField] private float _impactStrengthPerSize = 0.06f;
    [SerializeField] private float _cityLostStrength = 0.5f;

    private Vector3 _restPosition;
    private Coroutine _routine;
    private float _strength;
    private float _timeLeft;

    private void Awake()
    {
        _restPosition = transform.localPosition;
    }

    private void OnEnable()
    {
        Meteor.Impacted += OnMeteorImpacted;
        City.Destroyed += OnBigLoss;
        TurretHealth.Hit += OnBigLoss;
    }

    private void OnDisable()
    {
        Meteor.Impacted -= OnMeteorImpacted;
        City.Destroyed -= OnBigLoss;
        TurretHealth.Hit -= OnBigLoss;
    }

    private void OnMeteorImpacted(Vector3 position, float size)
    {
        Shake(size * _impactStrengthPerSize);
    }

    private void OnBigLoss()
    {
        Shake(_cityLostStrength);
    }

    private void Shake(float strength)
    {
        _strength = Mathf.Max(_strength, strength);
        _timeLeft = _duration;

        if (_routine == null)
        {
            _routine = StartCoroutine(ShakeRoutine());
        }
    }

    private IEnumerator ShakeRoutine()
    {
        while (_timeLeft > 0f)
        {
            float fade = _timeLeft / _duration;
            transform.localPosition = _restPosition + Random.insideUnitSphere * (_strength * fade);
            _timeLeft -= Time.deltaTime;
            yield return null;
        }

        transform.localPosition = _restPosition;
        _strength = 0f;
        _routine = null;
    }
}
