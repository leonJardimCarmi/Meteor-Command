using System.Collections;
using UnityEngine;

// An explosion made of two layers: a burst of sparks and an expanding ring. Both particle systems are
// set up in code, so the prefab only needs this script, a ParticleSystem and a material.
// Instances are pooled and return themselves when the explosion is over.
[RequireComponent(typeof(ParticleSystem))]
public class ExplosionEffect : MonoBehaviour
{
    [Header("Sparks")]
    [SerializeField] private int _particleCount = 30;
    [SerializeField] private float _lifetime = 0.7f;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _particleSize = 0.4f;
    [SerializeField] private float _gravity = 0.4f;

    [Header("Ring")]
    [SerializeField] private int _ringParticleCount = 28;
    [SerializeField] private float _ringLifetime = 0.4f;
    [SerializeField] private float _ringSpeed = 9f;
    [SerializeField] private float _ringParticleSize = 0.3f;

    private ParticleSystem _sparks;
    private ParticleSystem _ring;

    private void Awake()
    {
        _sparks = GetComponent<ParticleSystem>();
        _ring = CreateRing();
        ConfigureSparks();
        ConfigureRing();
    }

    public void Play(Color color, float size)
    {
        PlaySparks(color, size);
        PlayRing(Color.Lerp(color, Color.white, 0.5f), size);
        StartCoroutine(ReleaseWhenDone());
    }

    private void PlaySparks(Color color, float size)
    {
        ParticleSystem.MainModule main = _sparks.main;
        main.startColor = color;
        main.startSize = new ParticleSystem.MinMaxCurve(_particleSize * size * 0.5f, _particleSize * size);
        main.startSpeed = new ParticleSystem.MinMaxCurve(_speed * size * 0.4f, _speed * size);

        _sparks.Clear();
        _sparks.Play();
    }

    private void PlayRing(Color color, float size)
    {
        ParticleSystem.MainModule main = _ring.main;
        main.startColor = color;
        main.startSize = _ringParticleSize * size;
        main.startSpeed = _ringSpeed * size;

        _ring.Clear();
        _ring.Play();
    }

    private IEnumerator ReleaseWhenDone()
    {
        yield return new WaitForSeconds(Mathf.Max(_lifetime, _ringLifetime) + 0.2f);
        PoolManager.Instance.Release(gameObject);
    }

    // The ring is a child object, created inactive so it can be configured before it starts.
    private ParticleSystem CreateRing()
    {
        GameObject ringObject = new GameObject("Ring");
        ringObject.SetActive(false);
        ringObject.transform.SetParent(transform, false);

        ParticleSystem ring = ringObject.AddComponent<ParticleSystem>();
        ringObject.GetComponent<ParticleSystemRenderer>().sharedMaterial = GetComponent<ParticleSystemRenderer>().sharedMaterial;
        return ring;
    }

    private void ConfigureSparks()
    {
        ConfigureMain(_sparks, _lifetime, _gravity);
        ConfigureBurst(_sparks, _particleCount);
        ConfigureFade(_sparks);

        ParticleSystem.ShapeModule shape = _sparks.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.15f;
    }

    private void ConfigureRing()
    {
        ConfigureMain(_ring, _ringLifetime, 0f);
        ConfigureBurst(_ring, _ringParticleCount);
        ConfigureFade(_ring);

        ParticleSystem.ShapeModule shape = _ring.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.3f;
        shape.radiusThickness = 0f;

        _ring.gameObject.SetActive(true);
    }

    private void ConfigureMain(ParticleSystem particles, float lifetime, float gravity)
    {
        ParticleSystem.MainModule main = particles.main;
        main.playOnAwake = false;
        main.loop = false;
        main.startLifetime = lifetime;
        main.gravityModifier = gravity;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
    }

    private void ConfigureBurst(ParticleSystem particles, int count)
    {
        ParticleSystem.EmissionModule emission = particles.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)count) });
    }

    private void ConfigureFade(ParticleSystem particles)
    {
        ParticleSystem.ColorOverLifetimeModule fade = particles.colorOverLifetime;
        fade.enabled = true;
        fade.color = FadeOutGradient();

        ParticleSystem.SizeOverLifetimeModule shrink = particles.sizeOverLifetime;
        shrink.enabled = true;
        shrink.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, 0f));
    }

    private Gradient FadeOutGradient()
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) });
        return gradient;
    }
}
