using UnityEngine;

// Cuts the silence at the start of a clip and caps its length, so a sound plays the instant it is
// triggered and long files do not drag on. A capped clip gets a short fade out instead of a hard cut.
public static class AudioTrim
{
    private const float SilenceThreshold = 0.02f;
    private const float PreRollSeconds = 0.005f;
    private const float FadeSeconds = 0.25f;
    private const float MaxSilenceSeconds = 2f;

    public static AudioClip Prepare(AudioClip clip, float maxSeconds)
    {
        // Only the beginning is read, so a long file is not fully loaded into memory.
        int readFrames = Mathf.Min(clip.samples, Mathf.RoundToInt((maxSeconds + MaxSilenceSeconds) * clip.frequency));
        float[] samples = new float[readFrames * clip.channels];

        if (!clip.GetData(samples, 0))
        {
            return clip;
        }

        int startFrame = Mathf.Max(0, FirstLoudFrame(samples, clip.channels) - PreRollFrames(clip));
        int frames = Mathf.Min(readFrames - startFrame, Mathf.RoundToInt(maxSeconds * clip.frequency));
        bool isCapped = startFrame + frames < clip.samples;

        if (startFrame == 0 && !isCapped)
        {
            return clip;
        }

        return CopyFrames(clip, samples, startFrame, frames, isCapped);
    }

    private static int FirstLoudFrame(float[] samples, int channels)
    {
        for (int i = 0; i < samples.Length; i++)
        {
            if (Mathf.Abs(samples[i]) > SilenceThreshold)
            {
                return i / channels;
            }
        }

        return 0;
    }

    private static int PreRollFrames(AudioClip clip)
    {
        return Mathf.RoundToInt(PreRollSeconds * clip.frequency);
    }

    private static AudioClip CopyFrames(AudioClip clip, float[] samples, int startFrame, int frames, bool fadeOut)
    {
        float[] trimmed = new float[frames * clip.channels];
        System.Array.Copy(samples, startFrame * clip.channels, trimmed, 0, trimmed.Length);

        if (fadeOut)
        {
            FadeOut(trimmed, clip.channels, clip.frequency);
        }

        AudioClip result = AudioClip.Create(clip.name, frames, clip.channels, clip.frequency, false);
        result.SetData(trimmed, 0);
        return result;
    }

    private static void FadeOut(float[] samples, int channels, int frequency)
    {
        int fadeLength = Mathf.Min(samples.Length, Mathf.RoundToInt(FadeSeconds * frequency) * channels);
        int fadeStart = samples.Length - fadeLength;

        for (int i = 0; i < fadeLength; i++)
        {
            samples[fadeStart + i] *= 1f - (float)i / fadeLength;
        }
    }
}
