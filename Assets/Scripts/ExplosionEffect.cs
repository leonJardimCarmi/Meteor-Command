using System.Collections;
using UnityEngine;

// An explosion built from up to five particle layers: sparks and an expanding ring (always), and for a
// fiery explosion also a fireball, a puff of glowing smoke and flying embers. All the layers are set up in
// code and share the one glowing (additive) material, so the prefab only needs this script, a
// ParticleSystem and that material. Instances are pooled and return themselves when the explosion is over.
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

    [Header("Fireball")]
    [SerializeField] private int _fireballCount = 7;
    [SerializeField] private float _fireballLifetime = 0.55f;
    [SerializeField] private float _fireballSize = 2.4f;
    [SerializeField] private float _fireballSpeed = 1.4f;

    [Header("Smoke")]
    [SerializeField] private int _smokeCount = 14;
    [SerializeField] private float _smokeLifetime = 1.8f;
    [SerializeField] private float _smokeSize = 2.4f;
    [SerializeField] private float _smokeRise = -0.12f;
    [SerializeField] private Color _smokeColor = new Color(1f, 1f, 1f, 0.6f);

    [Header("Embers")]
    [SerializeField] private int _debrisCount = 9;
    [SerializeField] private float _debrisLifetime = 0.9f;
    [SerializeField] private float _debrisSize = 0.35f;
    [SerializeField] private float _debrisSpeed = 6f;
    [SerializeField] private float _debrisGravity = 2.2f;
    [SerializeField] private Color _debrisColor = new Color(1f, 0.55f, 0.2f, 1f);

    private ParticleSystem _sparks;
    private ParticleSystem _ring;
    private ParticleSystem _fireball;
    private ParticleSystem _smoke;
    private ParticleSystem _debris;

    private void Awake()
    {
        _sparks = GetComponent<ParticleSystem>();
        Material sparkMaterial = GetComponent<ParticleSystemRenderer>().sharedMaterial;

        _ring = CreateLayer("Ring", sparkMaterial);
        _fireball = CreateLayer("Fireball", sparkMaterial);
        _smoke = CreateLayer("Smoke", sparkMaterial);
        _debris = CreateLayer("Debris", sparkMaterial);

        ConfigureSparks();
        ConfigureRing();
        ConfigureFireball();
        ConfigureSmoke();
        ConfigureDebris();
    }

    public void Play(Color color, float size, bool isFiery)
    {
        PlaySparks(color, size);
        PlayRing(Color.Lerp(color, Color.white, 0.5f), size);

        if (isFiery)
        {
            PlayFireball(color, size);
            PlaySmoke(size);
            PlayDebris(size);
        }

        StartCoroutine(ReleaseWhenDone());
    }

    private void PlaySparks(Color color, float size)
    {
        ParticleSystem.MainModule main = _sparks.main;
        main.startColor = color;
        main.startSize = new ParticleSystem.MinMaxCurve(_particleSize * size * 0.5f, _particleSize * size);
        main.startSpeed = new ParticleSystem.MinMaxCurve(_speed * size * 0.4f, _speed * size);

        Restart(_sparks);
    }

    private void PlayRing(Color color, float size)
    {
        ParticleSystem.MainModule main = _ring.main;
        main.startColor = color;
        main.startSize = _ringParticleSize * size;
        main.startSpeed = _ringSpeed * size;

        Restart(_ring);
    }

    private void PlayFireball(Color color, float size)
    {
        ParticleSystem.MainModule main = _fireball.main;
        main.startColor = Color.Lerp(color, new Color(1f, 0.85f, 0.4f), 0.4f);
        main.startSize = new ParticleSystem.MinMaxCurve(_fireballSize * size * 0.6f, _fireballSize * size);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0f, _fireballSpeed * size);

        Restart(_fireball);
    }

    private void PlaySmoke(float size)
    {
        ParticleSystem.MainModule main = _smoke.main;
        main.startColor = _smokeColor;
        main.startSize = new ParticleSystem.MinMaxCurve(_smokeSize * size * 0.6f, _smokeSize * size);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.3f * size, 1.2f * size);

        Restart(_smoke);
    }

    private void PlayDebris(float size)
    {
        ParticleSystem.MainModule main = _debris.main;
        main.startColor = _debrisColor;
        main.startSize = new ParticleSystem.MinMaxCurve(_debrisSize * size * 0.5f, _debrisSize * size);
        main.startSpeed = new ParticleSystem.MinMaxCurve(_debrisSpeed * size * 0.4f, _debrisSpeed * size);

        Restart(_debris);
    }

    private void Restart(ParticleSystem particles)
    {
        particles.Clear();
        particles.Play();
    }

    private IEnumerator ReleaseWhenDone()
    {
        float longest = Mathf.Max(_lifetime, _ringLifetime, _fireballLifetime, _smokeLifetime, _debrisLifetime);
        yield return new WaitForSeconds(longest + 0.2f);
        PoolManager.Instance.Release(gameObject);
    }

    // A layer is a child object, created inactive so it can be configured before it starts.
    private ParticleSystem CreateLayer(string layerName, Material material)
    {
        GameObject layerObject = new GameObject(layerName);
        layerObject.SetActive(false);
        layerObject.transform.SetParent(transform, false);

        ParticleSystem layer = layerObject.AddComponent<ParticleSystem>();
        layerObject.GetComponent<ParticleSystemRenderer>().sharedMaterial = material;
        return layer;
    }

    private void ConfigureSparks()
    {
        ConfigureMain(_sparks, _lifetime, _gravity);
        ConfigureBurst(_sparks, _particleCount);
        ConfigureFade(_sparks, FadeOut());
        ConfigureSize(_sparks, AnimationCurve.Linear(0f, 1f, 1f, 0f));
        ConfigureShape(_sparks, ParticleSystemShapeType.Sphere, 0.15f);
    }

    private void ConfigureRing()
    {
        ConfigureMain(_ring, _ringLifetime, 0f);
        ConfigureBurst(_ring, _ringParticleCount);
        ConfigureFade(_ring, FadeOut());
        ConfigureSize(_ring, AnimationCurve.Linear(0f, 1f, 1f, 0f));
        ConfigureShape(_ring, ParticleSystemShapeType.Circle, 0.3f);

        ParticleSystem.ShapeModule shape = _ring.shape;
        shape.radiusThickness = 0f;

        _ring.gameObject.SetActive(true);
    }

    // The fireball grows quickly, then shrinks and fades: a hot flash at the start of the explosion.
    private void ConfigureFireball()
    {
        ConfigureMain(_fireball, _fireballLifetime, 0f);
        ConfigureBurst(_fireball, _fireballCount);
        ConfigureFade(_fireball, FadeOut());
        ConfigureSize(_fireball, GrowThenShrink(0.4f, 1f, 0.7f));
        ConfigureShape(_fireball, ParticleSystemShapeType.Sphere, 0.3f);

        _fireball.gameObject.SetActive(true);
    }

    // The smoke drifts upward while it grows, and fades in and out softly.
    private void ConfigureSmoke()
    {
        ConfigureMain(_smoke, _smokeLifetime, _smokeRise);
        ConfigureBurst(_smoke, _smokeCount);
        ConfigureFade(_smoke, SmokeFade());
        ConfigureSize(_smoke, AnimationCurve.Linear(0f, 0.6f, 1f, 1.6f));
        ConfigureShape(_smoke, ParticleSystemShapeType.Sphere, 0.4f);

        _smoke.gameObject.SetActive(true);
    }

    // Debris is thrown out fast and falls under gravity.
    private void ConfigureDebris()
    {
        ConfigureMain(_debris, _debrisLifetime, _debrisGravity);
        ConfigureBurst(_debris, _debrisCount);
        ConfigureFade(_debris, FadeOut());
        ConfigureSize(_debris, AnimationCurve.Linear(0f, 1f, 1f, 0.3f));
        ConfigureShape(_debris, ParticleSystemShapeType.Sphere, 0.2f);

        _debris.gameObject.SetActive(true);
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

    private void ConfigureShape(ParticleSystem particles, ParticleSystemShapeType type, float radius)
    {
        ParticleSystem.ShapeModule shape = particles.shape;
        shape.shapeType = type;
        shape.radius = radius;
    }

    private void ConfigureFade(ParticleSystem particles, Gradient gradient)
    {
        ParticleSystem.ColorOverLifetimeModule fade = particles.colorOverLifetime;
        fade.enabled = true;
        fade.color = gradient;
    }

    private void ConfigureSize(ParticleSystem particles, AnimationCurve curve)
    {
        ParticleSystem.SizeOverLifetimeModule size = particles.sizeOverLifetime;
        size.enabled = true;
        size.size = new ParticleSystem.MinMaxCurve(1f, curve);
    }

    private AnimationCurve GrowThenShrink(float start, float peak, float end)
    {
        return new AnimationCurve(
            new Keyframe(0f, start),
            new Keyframe(0.25f, peak),
            new Keyframe(1f, end));
    }

    private Gradient FadeOut()
    {
        return WhiteWithAlpha(new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f));
    }

    // Smoke is lit by the fire at first, so it starts orange and cools to grey-blue against the night sky.
    private Gradient SmokeFade()
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(new Color(1f, 0.62f, 0.35f), 0f),
                new GradientColorKey(new Color(0.66f, 0.62f, 0.66f), 0.25f),
                new GradientColorKey(new Color(0.42f, 0.45f, 0.56f), 1f)
            },
            new[]
            {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(1f, 0.12f),
                new GradientAlphaKey(0.75f, 0.5f),
                new GradientAlphaKey(0f, 1f)
            });
        return gradient;
    }

    private Gradient WhiteWithAlpha(params GradientAlphaKey[] alphaKeys)
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            alphaKeys);
        return gradient;
    }
}
