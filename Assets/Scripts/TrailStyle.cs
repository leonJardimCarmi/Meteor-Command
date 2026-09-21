using UnityEngine;

// Shared look for the trails behind meteors and interceptors: a tail that gets thinner and fades from a
// bright head, through warm colours, into dark ash. Built in code so it is the same everywhere.
public static class TrailStyle
{
    public static void Apply(TrailRenderer trail, float time, float width, Gradient colors)
    {
        trail.time = time;
        trail.widthCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
        trail.widthMultiplier = width;
        trail.colorGradient = colors;
        trail.Clear();
    }

    public static Gradient Flame()
    {
        return Fading(new Color(1f, 0.95f, 0.75f), new Color(1f, 0.6f, 0.15f), new Color(0.85f, 0.25f, 0.08f), new Color(0.25f, 0.22f, 0.24f));
    }

    public static Gradient Glow(Color color)
    {
        return Fading(Color.Lerp(color, Color.white, 0.7f), color, color * 0.4f, new Color(0.15f, 0.12f, 0.2f));
    }

    public static Gradient Smoke()
    {
        return Fading(Color.white, new Color(0.6f, 0.85f, 1f), new Color(0.35f, 0.5f, 0.75f), new Color(0.2f, 0.25f, 0.35f));
    }

    private static Gradient Fading(Color head, Color warm, Color cool, Color ash)
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(head, 0f),
                new GradientColorKey(warm, 0.2f),
                new GradientColorKey(cool, 0.55f),
                new GradientColorKey(ash, 1f)
            },
            new[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0.85f, 0.3f),
                new GradientAlphaKey(0.35f, 0.7f),
                new GradientAlphaKey(0f, 1f)
            });
        return gradient;
    }
}
