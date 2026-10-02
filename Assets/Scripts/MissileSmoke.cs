using UnityEngine;

// The thick white smoke a missile leaves in the sky: big soft puffs that swell, drift and thin out slowly. The
// emitter is not a child of the missile. A missile goes back into the pool the moment it arrives, and the smoke
// must stay behind and fade away by itself, so the missile only moves the emitter along while it flies.
public static class MissileSmoke
{
    private const float Lifetime = 2.1f;
    private const float PuffsPerUnit = 4.5f;
    private const int MaxPuffs = 400;

    public static ParticleSystem Create(Transform parent, Material material)
    {
        GameObject smokeObject = new GameObject("Missile Smoke");
        smokeObject.SetActive(false);
        smokeObject.transform.SetParent(parent, false);

        ParticleSystem smoke = smokeObject.AddComponent<ParticleSystem>();
        smokeObject.GetComponent<ParticleSystemRenderer>().sharedMaterial = material;

        ConfigureMain(smoke);
        ConfigureEmission(smoke);
        ConfigureLook(smoke);
        ConfigureMovement(smoke);

        smokeObject.SetActive(true);
        return smoke;
    }

    public static void Begin(ParticleSystem smoke, Vector3 position)
    {
        smoke.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        smoke.transform.position = position;
        smoke.Play();
    }

    public static void End(ParticleSystem smoke)
    {
        smoke.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private static void ConfigureMain(ParticleSystem smoke)
    {
        ParticleSystem.MainModule main = smoke.main;
        main.playOnAwake = false;
        main.loop = true;
        main.startLifetime = new ParticleSystem.MinMaxCurve(Lifetime * 0.8f, Lifetime);
        main.startSpeed = 0f;
        main.startSize = new ParticleSystem.MinMaxCurve(1.2f, 1.7f);
        main.startRotation = new ParticleSystem.MinMaxCurve(0f, Mathf.PI * 2f);
        main.gravityModifier = -0.02f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = MaxPuffs;
    }

    private static void ConfigureEmission(ParticleSystem smoke)
    {
        ParticleSystem.EmissionModule emission = smoke.emission;
        emission.rateOverTime = 0f;
        emission.rateOverDistance = PuffsPerUnit;
    }

    private static void ConfigureLook(ParticleSystem smoke)
    {
        ParticleSystem.ColorOverLifetimeModule color = smoke.colorOverLifetime;
        color.enabled = true;
        color.color = SmokeColors();

        ParticleSystem.SizeOverLifetimeModule size = smoke.sizeOverLifetime;
        size.enabled = true;
        size.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.EaseInOut(0f, 0.7f, 1f, 2.4f));

        ParticleSystem.RotationOverLifetimeModule rotation = smoke.rotationOverLifetime;
        rotation.enabled = true;
        rotation.z = new ParticleSystem.MinMaxCurve(-0.5f, 0.5f);
    }

    // A little turbulence makes the smoke curl and wander instead of staying in a straight, even tube.
    private static void ConfigureMovement(ParticleSystem smoke)
    {
        ParticleSystem.NoiseModule noise = smoke.noise;
        noise.enabled = true;
        noise.strength = 0.8f;
        noise.frequency = 0.3f;
        noise.scrollSpeed = 0.25f;
        noise.damping = true;
    }

    private static Gradient SmokeColors()
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(new Color(1f, 0.96f, 0.88f), 0f),
                new GradientColorKey(new Color(0.9f, 0.92f, 0.98f), 0.2f),
                new GradientColorKey(new Color(0.62f, 0.68f, 0.82f), 1f)
            },
            new[]
            {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(0.8f, 0.05f),
                new GradientAlphaKey(0.5f, 0.4f),
                new GradientAlphaKey(0f, 1f)
            });
        return gradient;
    }
}
