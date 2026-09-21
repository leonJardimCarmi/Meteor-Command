using System;
using System.Collections;
using UnityEngine;

// A city building. When it is destroyed it sinks into a low pile of rubble, and smoke rises from the ruin.
public class City : MonoBehaviour
{
    public static event Action Destroyed;

    [SerializeField] private Material _rubbleMaterial;
    [SerializeField] private float _rubbleHeight = 0.4f;
    [SerializeField] private float _collapseSeconds = 0.4f;
    [SerializeField] private Material _smokeMaterial;
    [SerializeField] private float _smokeSeconds = 14f;

    private MeshRenderer _renderer;
    private ParticleSystem _smoke;

    public bool IsAlive { get; private set; } = true;

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();

        if (_smokeMaterial != null)
        {
            _smoke = SmokePlume.Create(transform, _smokeMaterial, new Vector3(0f, 0.5f, 0f));
        }
    }

    public void Collapse()
    {
        IsAlive = false;
        StartCoroutine(Crumble());
        StartCoroutine(Smoulder());
        Destroyed?.Invoke();
    }

    private IEnumerator Crumble()
    {
        if (_rubbleMaterial != null)
        {
            _renderer.sharedMaterial = _rubbleMaterial;
        }

        float startHeight = transform.localScale.y;

        for (float progress = 0f; progress < 1f; progress += Time.deltaTime / _collapseSeconds)
        {
            SetHeight(Mathf.Lerp(startHeight, _rubbleHeight, progress));
            yield return null;
        }

        SetHeight(_rubbleHeight);
    }

    // The ruin smokes for a while, then the smoke stops and the last puffs drift away.
    private IEnumerator Smoulder()
    {
        if (_smoke == null)
        {
            yield break;
        }

        _smoke.Play();
        yield return new WaitForSeconds(_smokeSeconds);
        _smoke.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    // The base stays on the ground while the height changes.
    private void SetHeight(float height)
    {
        Vector3 scale = transform.localScale;
        scale.y = height;
        transform.localScale = scale;

        Vector3 position = transform.position;
        position.y = height * 0.5f;
        transform.position = position;
    }
}
