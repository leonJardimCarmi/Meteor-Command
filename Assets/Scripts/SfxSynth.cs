using UnityEngine;

// Builds simple retro sound effects in code, so the game has sound without any audio files.
public static class SfxSynth
{
    private const int SampleRate = 44100;

    private enum WaveShape
    {
        Sine,
        Square
    }

    public static AudioClip Launch()
    {
        return ToClip("Launch", Sweep(300f, 1100f, 0.25f, 0.3f, WaveShape.Sine));
    }

    public static AudioClip Blast()
    {
        float[] rumble = Noise(0.7f, 0.6f, 0.12f);
        float[] thump = Sweep(140f, 40f, 0.7f, 0.5f, WaveShape.Sine);
        return ToClip("Blast", Mix(rumble, thump));
    }

    public static AudioClip Kill()
    {
        return ToClip("Kill", Sweep(900f, 250f, 0.12f, 0.25f, WaveShape.Square));
    }

    public static AudioClip Impact()
    {
        return ToClip("Impact", Noise(0.35f, 0.5f, 0.2f));
    }

    public static AudioClip CityLost()
    {
        return ToClip("CityLost", Sweep(320f, 60f, 0.8f, 0.3f, WaveShape.Square));
    }

    public static AudioClip WaveStart()
    {
        float[] low = Sweep(440f, 440f, 0.15f, 0.25f, WaveShape.Square);
        float[] high = Sweep(660f, 660f, 0.25f, 0.25f, WaveShape.Square);
        return ToClip("WaveStart", Join(low, high));
    }

    public static AudioClip GameOver()
    {
        return ToClip("GameOver", Sweep(400f, 45f, 1.4f, 0.35f, WaveShape.Square));
    }

    private static float[] Sweep(float startHz, float endHz, float seconds, float volume, WaveShape shape)
    {
        float[] samples = new float[(int)(seconds * SampleRate)];
        float phase = 0f;

        for (int i = 0; i < samples.Length; i++)
        {
            float progress = (float)i / samples.Length;
            phase += Mathf.Lerp(startHz, endHz, progress) / SampleRate;
            samples[i] = Wave(phase, shape) * volume * (1f - progress);
        }

        return samples;
    }

    private static float[] Noise(float seconds, float volume, float smoothing)
    {
        float[] samples = new float[(int)(seconds * SampleRate)];
        float smoothed = 0f;

        for (int i = 0; i < samples.Length; i++)
        {
            float progress = (float)i / samples.Length;
            float raw = Random.value * 2f - 1f;
            smoothed += (raw - smoothed) * smoothing;
            float fade = (1f - progress) * (1f - progress);
            samples[i] = Mathf.Clamp(smoothed * volume * fade * 4f, -1f, 1f);
        }

        return samples;
    }

    private static float Wave(float phase, WaveShape shape)
    {
        float sine = Mathf.Sin(phase * 2f * Mathf.PI);
        return shape == WaveShape.Sine ? sine : Mathf.Sign(sine);
    }

    private static float[] Mix(float[] first, float[] second)
    {
        float[] mixed = new float[Mathf.Max(first.Length, second.Length)];

        for (int i = 0; i < mixed.Length; i++)
        {
            float a = i < first.Length ? first[i] : 0f;
            float b = i < second.Length ? second[i] : 0f;
            mixed[i] = Mathf.Clamp(a + b, -1f, 1f);
        }

        return mixed;
    }

    private static float[] Join(float[] first, float[] second)
    {
        float[] joined = new float[first.Length + second.Length];
        first.CopyTo(joined, 0);
        second.CopyTo(joined, first.Length);
        return joined;
    }

    private static AudioClip ToClip(string name, float[] samples)
    {
        AudioClip clip = AudioClip.Create(name, samples.Length, 1, SampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}
