using System;
using UnityEngine;

// Draws a small picture in code, so an effect like the crosshair or the danger glow needs no image file.
public static class ProceduralSprite
{
    // alphaAt is given a position inside the picture, from -1 to 1 on both axes, and returns how solid it is there.
    // The picture is white, so the colour comes from whatever shows it.
    public static Sprite Create(int size, Func<float, float, float> alphaAt, float pixelsPerUnit = 100f)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.wrapMode = TextureWrapMode.Clamp;
        Color[] pixels = new Color[size * size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float offsetX = (x + 0.5f) / size * 2f - 1f;
                float offsetY = (y + 0.5f) / size * 2f - 1f;
                pixels[y * size + x] = new Color(1f, 1f, 1f, alphaAt(offsetX, offsetY));
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), pixelsPerUnit);
    }

    // 0 up to edge0, 1 from edge1 on, and a smooth curve in between. Mathf.SmoothStep is not the same thing: it
    // blends between two values, so it cannot be used to soften an edge.
    public static float SoftEdge(float edge0, float edge1, float value)
    {
        float t = Mathf.Clamp01((value - edge0) / (edge1 - edge0));
        return t * t * (3f - 2f * t);
    }
}
