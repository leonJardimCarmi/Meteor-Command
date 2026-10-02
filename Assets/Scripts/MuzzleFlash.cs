using System.Collections;
using UnityEngine;

// The flash at the barrel when the battery fires: a bright flare, a spray of sparks and a short orange light.
// It sits on the muzzle point and is built in code, so it only needs the glowing spark material.
public class MuzzleFlash : MonoBehaviour
{
    private const float LightIntensity = 18f;
    private const float LightRange = 9f;
    private const float LightSeconds = 0.12f;

    private static readonly Color FlashColor = new Color(1f, 0.8f, 0.45f);

    [SerializeField] private Material _sparkMaterial;

    private ParticleSystem _flare;
    private ParticleSystem _sparks;
    private Light _light;
    private Coroutine _lightRoutine;

    private void Awake()
    {
        _flare = CreateBurst("Flare", 1, 1.6f, 2f, 0f, 0.07f, 0.08f);
        _sparks = CreateBurst("Sparks", 16, 0.1f, 0.25f, 5f, 11f, 0.3f);

        _light = gameObject.AddComponent<Light>();
        _light.type = LightType.Point;
        _light.color = FlashColor;
        _light.range = LightRange;
        _light.shadows = LightShadows.None;
        _light.enabled = false;
    }

    private void OnEnable()
    {
        Battery.Fired += Flash;
    }

    private void OnDisable()
    {
        Battery.Fired -= Flash;
    }

    private void Flash()
    {
        _flare.Play();
        _sparks.Play();

        if (_lightRoutine != null)
        {
            StopCoroutine(_lightRoutine);
        }

        _lightRoutine = StartCoroutine(FlashLight());
    }

    private IEnumerator FlashLight()
    {
        _light.enabled = true;

        for (float progress = 0f; progress < 1f; progress += Time.deltaTime / LightSeconds)
        {
            _light.intensity = LightIntensity * (1f - progress);
            yield return null;
        }

        _light.enabled = false;
        _lightRoutine = null;
    }

    private ParticleSystem CreateBurst(string objectName, int count, float minSize, float maxSize, float minSpeed, float maxSpeed, float lifetime)
    {
        GameObject burstObject = new GameObject(objectName);
        burstObject.SetActive(false);
        burstObject.transform.SetParent(transform, false);

        ParticleSystem burst = burstObject.AddComponent<ParticleSystem>();
        burstObject.GetComponent<ParticleSystemRenderer>().sharedMaterial = _sparkMaterial;

        ParticleSystem.MainModule main = burst.main;
        main.playOnAwake = false;
        main.loop = false;
        main.startLifetime = lifetime;
        main.startSize = new ParticleSystem.MinMaxCurve(minSize, maxSize);
        main.startSpeed = new ParticleSystem.MinMaxCurve(minSpeed, maxSpeed);
        main.startColor = FlashColor;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        ParticleSystem.EmissionModule emission = burst.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)count) });

        ParticleSystem.ShapeModule shape = burst.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.1f;

        ParticleSystem.SizeOverLifetimeModule size = burst.sizeOverLifetime;
        size.enabled = true;
        size.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, 0f));

        burstObject.SetActive(true);
        return burst;
    }
}
