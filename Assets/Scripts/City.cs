using System;
using System.Collections;
using UnityEngine;

// A city building. When it is destroyed it shakes and sinks, breaks into a pile of rubble, and then burns for
// a while: smoke and embers rise from the ruin and a flickering light shines on it.
public class City : MonoBehaviour
{
    private const float ShakeDistance = 0.06f;
    private const int CollapseDustPuffs = 10;

    public static event Action Destroyed;

    [Header("Collapse")]
    [SerializeField] private Material _rubbleMaterial;
    [SerializeField] private Mesh _rubbleMesh;
    [SerializeField] private float _sinkHeight = 0.9f;
    [SerializeField] private float _collapseSeconds = 0.4f;

    [Header("Ruin")]
    [SerializeField] private Material _smokeMaterial;
    [SerializeField] private float _smokeSeconds = 14f;
    [SerializeField] private float _smokeHeight = 0.4f;

    private MeshFilter _meshFilter;
    private MeshRenderer _renderer;
    private ParticleSystem _smoke;
    private RuinFire _fire;
    private Vector3 _intactPosition;
    private Vector3 _intactScale;

    public bool IsAlive { get; private set; } = true;

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _renderer = GetComponent<MeshRenderer>();
        _intactPosition = transform.position;
        _intactScale = transform.localScale;
        _fire = RuinFire.Create(transform);

        if (_smokeMaterial != null)
        {
            _smoke = SmokePlume.Create(transform, _smokeMaterial);
        }
    }

    public void Collapse()
    {
        IsAlive = false;
        StartCoroutine(FallToRuin());
        Destroyed?.Invoke();
    }

    private IEnumerator FallToRuin()
    {
        yield return StartCoroutine(Sink());
        ShowRubble();
        yield return StartCoroutine(Burn());
    }

    // The building shakes and sinks toward the ground.
    private IEnumerator Sink()
    {
        for (float progress = 0f; progress < 1f; progress += Time.deltaTime / _collapseSeconds)
        {
            float height = Mathf.Lerp(_intactScale.y, _sinkHeight, progress * progress);
            float shake = UnityEngine.Random.Range(-ShakeDistance, ShakeDistance) * (1f - progress);

            SetShape(height, shake);
            yield return null;
        }
    }

    // The base stays on the ground while the height changes.
    private void SetShape(float height, float shake)
    {
        Vector3 scale = _intactScale;
        scale.y = height;
        transform.localScale = scale;
        transform.position = new Vector3(_intactPosition.x + shake, height * 0.5f, _intactPosition.z);
    }

    // The pile of rubble has the size of the building, and each ruin is turned a different way.
    private void ShowRubble()
    {
        _meshFilter.sharedMesh = _rubbleMesh != null ? _rubbleMesh : RubbleMesh.Shared;

        if (_rubbleMaterial != null)
        {
            _renderer.sharedMaterial = _rubbleMaterial;
        }

        transform.localScale = _intactScale;
        transform.SetPositionAndRotation(_intactPosition, Quaternion.Euler(0f, 90f * UnityEngine.Random.Range(0, 4), 0f));
    }

    // The ruin burns for a while, then the flames die down and the last puffs of smoke drift away.
    private IEnumerator Burn()
    {
        Vector3 ruinPoint = new Vector3(_intactPosition.x, _smokeHeight, _intactPosition.z);
        _fire.Ignite(ruinPoint, _smokeSeconds);

        if (_smoke == null)
        {
            yield break;
        }

        _smoke.transform.position = ruinPoint;
        _smoke.Play();
        _smoke.Emit(CollapseDustPuffs);
        yield return new WaitForSeconds(_smokeSeconds);
        _smoke.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
}
