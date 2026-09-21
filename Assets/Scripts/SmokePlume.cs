using UnityEngine;

// The smoke and embers of a burning ruin. A column of dark smoke rises slowly from one spot, and a few glowing
// embers float up with it. The emitters are child objects created in code, so an object only needs a material
// to get a plume. Puffs are made over time, drift up, grow and fade, and are lit warm by the fire at the bottom.
public static class SmokePlume
{
    private const float Lifetime = 4f;
    private const float PuffsPerSecond = 8f;
    private const float RiseSpeed = 1.3f;
    private const float EmbersPerSecond = 4f;

    public static ParticleSystem Create(Transform parent, Material material)
    {
        GameObject plumeObject = new GameObject("Smoke Plume");
        plumeObject.SetActive(false);
        plumeObject.transform.SetParent(parent, false);
        plumeObject.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);

        ParticleSystem plume = plumeObject.AddComponent<ParticleSystem>();
        plumeObject.GetComponent<ParticleSystemRenderer>().sharedMaterial = material;

        ConfigureMain(plume);
        ConfigureShape(plume);
        ConfigureLook(plume);
        AddEmbers(plumeObject.transform, material);

        plumeObject.SetActive(true);
        return plume;
    }

    private static void ConfigureMain(ParticleSystem plume)
    {
        ParticleSystem.MainModule main = plume.main;
        main.playOnAwake = false;
        main.loop = true;
        main.startLifetime = Lifetime;
        main.startSpeed = RiseSpeed;
        main.startSize = new ParticleSystem.MinMaxCurve(1.1f, 1.7f);
        main.gravityModifier = -0.03f;
        main.scalingMode = ParticleSystemScalingMode.Local;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        ParticleSystem.EmissionModule emission = plume.emission;
        emission.rateOverTime = PuffsPerSecond;
    }

    // A narrow cone pointing up, since the emitter is turned to face the sky.
    private static void ConfigureShape(ParticleSystem plume)
    {
        ParticleSystem.ShapeModule shape = plume.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 10f;
        shape.radius = 0.5f;
    }

    private static void ConfigureLook(ParticleSystem plume)
    {
        ParticleSystem.ColorOverLifetimeModule color = plume.colorOverLifetime;
        color.enabled = true;
        color.color = PlumeColors();

        ParticleSystem.SizeOverLifetimeModule size = plume.sizeOverLifetime;
        size.enabled = true;
        size.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 0.6f, 1f, 2.6f));
    }

    private static Gradient PlumeColors()
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(new Color(0.95f, 0.5f, 0.28f), 0f),
                new GradientColorKey(new Color(0.5f, 0.48f, 0.52f), 0.2f),
                new GradientColorKey(new Color(0.3f, 0.3f, 0.36f), 1f)
            },
            new[]
            {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(0.7f, 0.15f),
                new GradientAlphaKey(0.45f, 0.6f),
                new GradientAlphaKey(0f, 1f)
            });
        return gradient;
    }

    // Small glowing sparks that rise from the fire faster than the smoke, and cool down as they go. They are a
    // child of the plume, so they play and stop together with it.
    private static void AddEmbers(Transform plume, Material material)
    {
        GameObject embersObject = new GameObject("Embers");
        embersObject.SetActive(false);
        embersObject.transform.SetParent(plume, false);

        ParticleSystem embers = embersObject.AddComponent<ParticleSystem>();
        embersObject.GetComponent<ParticleSystemRenderer>().sharedMaterial = material;

        ParticleSystem.MainModule main = embers.main;
        main.playOnAwake = false;
        main.loop = true;
        main.startLifetime = new ParticleSystem.MinMaxCurve(1.2f, 2.2f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.8f, 2f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.12f, 0.24f);
        main.scalingMode = ParticleSystemScalingMode.Local;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        ParticleSystem.EmissionModule emission = embers.emission;
        emission.rateOverTime = EmbersPerSecond;

        ParticleSystem.ShapeModule shape = embers.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 30f;
        shape.radius = 0.8f;

        ParticleSystem.ColorOverLifetimeModule color = embers.colorOverLifetime;
        color.enabled = true;
        color.color = EmberColors();

        ParticleSystem.SizeOverLifetimeModule size = embers.sizeOverLifetime;
        size.enabled = true;
        size.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, 0.3f));

        embersObject.SetActive(true);
    }

    private static Gradient EmberColors()
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(new Color(1f, 0.85f, 0.45f), 0f),
                new GradientColorKey(new Color(1f, 0.45f, 0.12f), 0.4f),
                new GradientColorKey(new Color(0.55f, 0.12f, 0.08f), 1f)
            },
            new[]
            {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(1f, 0.1f),
                new GradientAlphaKey(0.8f, 0.6f),
                new GradientAlphaKey(0f, 1f)
            });
        return gradient;
    }
}
