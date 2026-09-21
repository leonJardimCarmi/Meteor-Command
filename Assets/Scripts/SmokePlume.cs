using UnityEngine;

// A column of dark smoke that rises slowly from one spot, like a burning ruin. The emitter is a child object
// created in code, so an object only needs a material to get a plume. Puffs are made over time, drift up,
// grow and fade, and are lit warm by the fire at the bottom.
public static class SmokePlume
{
    private const float Lifetime = 4f;
    private const float PuffsPerSecond = 5f;
    private const float RiseSpeed = 1.3f;

    public static ParticleSystem Create(Transform parent, Material material, Vector3 localPosition)
    {
        GameObject plumeObject = new GameObject("Smoke Plume");
        plumeObject.SetActive(false);
        plumeObject.transform.SetParent(parent, false);
        plumeObject.transform.localPosition = localPosition;
        plumeObject.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);

        ParticleSystem plume = plumeObject.AddComponent<ParticleSystem>();
        plumeObject.GetComponent<ParticleSystemRenderer>().sharedMaterial = material;

        ConfigureMain(plume);
        ConfigureShape(plume);
        ConfigureLook(plume);

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
        main.startSize = new ParticleSystem.MinMaxCurve(0.9f, 1.4f);
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
                new GradientColorKey(new Color(0.34f, 0.32f, 0.36f), 0.2f),
                new GradientColorKey(new Color(0.2f, 0.2f, 0.26f), 1f)
            },
            new[]
            {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(0.6f, 0.15f),
                new GradientAlphaKey(0.4f, 0.6f),
                new GradientAlphaKey(0f, 1f)
            });
        return gradient;
    }
}
