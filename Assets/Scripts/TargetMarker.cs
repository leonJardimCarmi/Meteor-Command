using UnityEngine;

// An X in a ring on the spot where a missile will go off, shown from the moment it is fired until it arrives.
// It pulses a little so it is easy to spot. The marker is drawn in code, so there is no picture file to import.
public class TargetMarker : MonoBehaviour
{
    private const int TextureSize = 128;
    private const float PixelsPerUnit = 64f;
    private const float RingRadius = 0.72f;
    private const float PulseSpeed = 14f;
    private const float PulseAmount = 0.12f;

    private static readonly Color MarkerColor = new Color(0.7f, 1f, 1f, 1f);

    // The material is the soft alpha-blended sprite material also used by the smoke. A sprite without a material
    // of its own is drawn as a plain solid square in this project.
    public static TargetMarker Create(Transform parent, Material material)
    {
        GameObject markerObject = new GameObject("Target Marker");
        markerObject.SetActive(false);
        markerObject.transform.SetParent(parent, false);

        SpriteRenderer renderer = markerObject.AddComponent<SpriteRenderer>();
        renderer.sprite = ProceduralSprite.Create(TextureSize, MarkerAlpha, PixelsPerUnit);
        renderer.sharedMaterial = material;
        renderer.color = MarkerColor;

        return markerObject.AddComponent<TargetMarker>();
    }

    public void Show(Vector3 position)
    {
        transform.position = position;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        transform.localScale = Vector3.one * (1f + PulseAmount * Mathf.Sin(Time.time * PulseSpeed));
    }

    // A ring with a thin X inside it.
    private static float MarkerAlpha(float offsetX, float offsetY)
    {
        float distance = Mathf.Sqrt(offsetX * offsetX + offsetY * offsetY);
        float ring = 1f - ProceduralSprite.SoftEdge(0f, 0.09f, Mathf.Abs(distance - RingRadius));

        float diagonal = Mathf.Abs(Mathf.Abs(offsetX) - Mathf.Abs(offsetY));
        float cross = (1f - ProceduralSprite.SoftEdge(0.04f, 0.12f, diagonal)) * (1f - ProceduralSprite.SoftEdge(RingRadius * 0.75f, RingRadius * 0.9f, distance));

        return Mathf.Clamp01(Mathf.Max(ring, cross));
    }
}
