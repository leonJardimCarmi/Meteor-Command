using UnityEngine;

// A trail of soft smoke puffs that a fast moving object leaves behind. The puffs stay where they were made,
// start warm because the fire lights them, then turn grey, grow and fade. The emitter is a child object
// created in code, so an object only needs a material to get a smoke trail.
public static class SmokeTrail
{
    private const float Lifetime = 1.4f;
    private const float PuffsPerUnit = 2.2f;

    public static ParticleSystem Create(Transform parent, Material material)
    {
        GameObject smokeObject = new GameObject("Smoke Trail");
        smokeObject.SetActive(false);
        smokeObject.transform.SetParent(parent, false);

        ParticleSystem smoke = smokeObject.AddComponent<ParticleSystem>();
        smokeObject.GetComponent<ParticleSystemRenderer>().sharedMaterial = material;

        ConfigureMain(smoke);
        ConfigureEmission(smoke);
        ConfigureLook(smoke);

        smokeObject.SetActive(true);
        return smoke;
    }

    // Starts the trail again with puffs sized for the object, so a big meteor leaves a bigger trail.
    public static void Restart(ParticleSystem smoke, float size)
    {
        ParticleSystem.MainModule main = smoke.main;
        main.startSize = new ParticleSystem.MinMaxCurve(size * 0.35f, size * 0.55f);

        smoke.Clear();
        smoke.Play();
    }

    private static void ConfigureMain(ParticleSystem smoke)
    {
        ParticleSystem.MainModule main = smoke.main;
        main.playOnAwake = false;
        main.loop = true;
        main.startLifetime = Lifetime;
        main.startSpeed = 0f;
        main.gravityModifier = -0.03f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
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
        size.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 0.7f, 1f, 2.2f));
    }

    private static Gradient SmokeColors()
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(new Color(1f, 0.6f, 0.3f), 0f),
                new GradientColorKey(new Color(0.55f, 0.5f, 0.5f), 0.3f),
                new GradientColorKey(new Color(0.28f, 0.28f, 0.34f), 1f)
            },
            new[]
            {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(0.7f, 0.1f),
                new GradientAlphaKey(0.45f, 0.5f),
                new GradientAlphaKey(0f, 1f)
            });
        return gradient;
    }
}
