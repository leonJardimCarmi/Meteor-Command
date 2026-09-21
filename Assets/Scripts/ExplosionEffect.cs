using System.Collections;
using UnityEngine;

// A burst of sparks. The particle system is set up in code, so the prefab only needs this script,
// a ParticleSystem and a material. Instances are pooled and return themselves when the burst is over.
[RequireComponent(typeof(ParticleSystem))]
public class ExplosionEffect : MonoBehaviour
{
    [SerializeField] private int _particleCount = 30;
    [SerializeField] private float _lifetime = 0.7f;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _particleSize = 0.4f;
    [SerializeField] private float _gravity = 0.4f;

    private ParticleSystem _particles;

    private void Awake()
    {
        _particles = GetComponent<ParticleSystem>();
        Configure();
    }

    public void Play(Color color, float size)
    {
        ParticleSystem.MainModule main = _particles.main;
        main.startColor = color;
        main.startSize = new ParticleSystem.MinMaxCurve(_particleSize * size * 0.5f, _particleSize * size);
        main.startSpeed = new ParticleSystem.MinMaxCurve(_speed * size * 0.4f, _speed * size);

        _particles.Clear();
        _particles.Play();
        StartCoroutine(ReleaseWhenDone());
    }

    private IEnumerator ReleaseWhenDone()
    {
        yield return new WaitForSeconds(_lifetime + 0.2f);
        PoolManager.Instance.Release(gameObject);
    }

    private void Configure()
    {
        ConfigureMain();
        ConfigureEmission();
        ConfigureShape();
        ConfigureFade();
    }

    private void ConfigureMain()
    {
        ParticleSystem.MainModule main = _particles.main;
        main.playOnAwake = false;
        main.loop = false;
        main.startLifetime = _lifetime;
        main.gravityModifier = _gravity;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
    }

    private void ConfigureEmission()
    {
        ParticleSystem.EmissionModule emission = _particles.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)_particleCount) });
    }

    private void ConfigureShape()
    {
        ParticleSystem.ShapeModule shape = _particles.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.15f;
    }

    private void ConfigureFade()
    {
        ParticleSystem.ColorOverLifetimeModule fade = _particles.colorOverLifetime;
        fade.enabled = true;
        fade.color = FadeOutGradient();

        ParticleSystem.SizeOverLifetimeModule shrink = _particles.sizeOverLifetime;
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
