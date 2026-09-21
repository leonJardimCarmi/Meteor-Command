using System;
using System.Collections;
using UnityEngine;

// A city building. When it is destroyed it sinks into a low pile of rubble instead of disappearing.
public class City : MonoBehaviour
{
    public static event Action Destroyed;

    [SerializeField] private Material _rubbleMaterial;
    [SerializeField] private float _rubbleHeight = 0.4f;
    [SerializeField] private float _collapseSeconds = 0.4f;

    private MeshRenderer _renderer;

    public bool IsAlive { get; private set; } = true;

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
    }

    public void Collapse()
    {
        IsAlive = false;
        StartCoroutine(Crumble());
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
