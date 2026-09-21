using System.Collections;
using UnityEngine;

// The glow of a burning ruin: a flickering orange point light that burns for a while and then dies down.
// The light is created in code, so a city only needs to ask for it.
public class RuinFire : MonoBehaviour
{
    private const float Intensity = 10f;
    private const float Range = 8f;
    private const float FlickerSpeed = 5f;
    private const float DimmestFlicker = 0.55f;
    private const float FadeSeconds = 2f;

    // The light hangs in front of the ruin and a little above it, otherwise it would shine into the pile.
    private static readonly Vector3 Offset = new Vector3(0f, 1f, -1.8f);
    private static readonly Color FireColor = new Color(1f, 0.5f, 0.18f);

    private Light _light;
    private float _seed;

    public static RuinFire Create(Transform parent)
    {
        GameObject fireObject = new GameObject("Ruin Fire");
        fireObject.transform.SetParent(parent, false);
        return fireObject.AddComponent<RuinFire>();
    }

    private void Awake()
    {
        _light = gameObject.AddComponent<Light>();
        _light.type = LightType.Point;
        _light.color = FireColor;
        _light.range = Range;
        _light.shadows = LightShadows.None;
        _light.enabled = false;
        _seed = Random.value * 100f;
    }

    public void Ignite(Vector3 ruinPoint, float burnSeconds)
    {
        transform.position = ruinPoint + Offset;
        StartCoroutine(Burn(burnSeconds));
    }

    private IEnumerator Burn(float burnSeconds)
    {
        float totalSeconds = burnSeconds + FadeSeconds;
        _light.enabled = true;

        for (float elapsed = 0f; elapsed < totalSeconds; elapsed += Time.deltaTime)
        {
            float fade = Mathf.Clamp01((totalSeconds - elapsed) / FadeSeconds);
            float flicker = Mathf.Lerp(DimmestFlicker, 1f, Mathf.PerlinNoise(_seed, elapsed * FlickerSpeed));

            _light.intensity = Intensity * flicker * fade;
            yield return null;
        }

        _light.enabled = false;
    }
}
