using System.Collections;
using UnityEngine;

// Leaves a mark where a meteor lands on the ground: a dark scorch mark that fades away slowly, a hot glow that
// cools down quickly, and a ring that spreads out from the spot. Together they show that the meteor really
// landed, even when it was harmless. A fixed set of marks is used one after another, so the oldest mark makes
// way for the newest. The pictures are drawn in code.
public class GroundScars : MonoBehaviour
{
    private const int TextureSize = 128;
    private const float GroundHeight = 0.03f;
    private const float DepthSquash = 0.55f;
    private const float RingSpread = 1.6f;

    [SerializeField] private Material _material;
    [SerializeField] private int _scarCount = 12;
    [SerializeField] private float _widthPerMeteorSize = 1.2f;
    [SerializeField] private float _scarSeconds = 10f;
    [SerializeField] private float _glowSeconds = 3.5f;
    [SerializeField] private float _ringSeconds = 0.6f;
    [SerializeField] private Color _scarColor = new Color(0.08f, 0.045f, 0.03f, 0.9f);
    [SerializeField] private Color _glowColor = new Color(1f, 0.5f, 0.18f, 0.8f);
    [SerializeField] private Color _ringColor = new Color(1f, 0.8f, 0.55f, 0.8f);

    private Scar[] _scars;
    private int _next;

    private void Awake()
    {
        Sprite scarSprite = ProceduralSprite.Create(TextureSize, ScarAlpha);
        Sprite glowSprite = ProceduralSprite.Create(TextureSize, GlowAlpha);
        Sprite ringSprite = ProceduralSprite.Create(TextureSize, RingAlpha);

        _scars = new Scar[_scarCount];

        for (int i = 0; i < _scarCount; i++)
        {
            _scars[i] = new Scar
            {
                Mark = CreateLayer("Scar", scarSprite, 0),
                Glow = CreateLayer("Glow", glowSprite, 1),
                Ring = CreateLayer("Ring", ringSprite, 2)
            };
        }
    }

    private void OnEnable()
    {
        Meteor.Impacted += OnMeteorImpacted;
    }

    private void OnDisable()
    {
        Meteor.Impacted -= OnMeteorImpacted;
    }

    private void OnMeteorImpacted(Vector3 position, float size)
    {
        Scar scar = _scars[_next];
        _next = (_next + 1) % _scars.Length;

        if (scar.Routine != null)
        {
            StopCoroutine(scar.Routine);
        }

        scar.Routine = StartCoroutine(Show(scar, new Vector3(position.x, GroundHeight, position.z), size * _widthPerMeteorSize));
    }

    private IEnumerator Show(Scar scar, Vector3 position, float width)
    {
        Place(scar.Mark, position, width);
        Place(scar.Glow, position, width * 0.7f);
        Place(scar.Ring, position, width);

        for (float elapsed = 0f; elapsed < _scarSeconds; elapsed += Time.deltaTime)
        {
            scar.Mark.color = Faded(_scarColor, Mathf.Clamp01(2f * (1f - elapsed / _scarSeconds)));
            scar.Glow.color = Faded(_glowColor, 1f - Mathf.Clamp01(elapsed / _glowSeconds));
            ShowRing(scar.Ring, position, width, elapsed / _ringSeconds);
            yield return null;
        }

        Hide(scar);
        scar.Routine = null;
    }

    private void ShowRing(SpriteRenderer ring, Vector3 position, float width, float progress)
    {
        if (progress >= 1f)
        {
            ring.color = Color.clear;
            return;
        }

        float spread = Mathf.Lerp(0.3f, RingSpread, progress);
        ring.transform.localScale = new Vector3(width * spread, width * spread * DepthSquash, 1f);
        ring.color = Faded(_ringColor, 1f - progress);
    }

    private static void Place(SpriteRenderer layer, Vector3 position, float width)
    {
        layer.transform.position = position;
        layer.transform.localScale = new Vector3(width, width * DepthSquash, 1f);
    }

    private static void Hide(Scar scar)
    {
        scar.Mark.color = Color.clear;
        scar.Glow.color = Color.clear;
        scar.Ring.color = Color.clear;
    }

    private static Color Faded(Color color, float amount)
    {
        color.a *= amount;
        return color;
    }

    private SpriteRenderer CreateLayer(string layerName, Sprite sprite, int order)
    {
        GameObject layerObject = new GameObject(layerName);
        layerObject.transform.SetParent(transform, false);
        layerObject.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        SpriteRenderer layer = layerObject.AddComponent<SpriteRenderer>();
        layer.sprite = sprite;
        layer.sharedMaterial = _material;
        layer.sortingOrder = order;
        layer.color = Color.clear;
        return layer;
    }

    // A rough blob with a soft edge, so each scorch mark is not a perfect circle.
    private static float ScarAlpha(float offsetX, float offsetY)
    {
        float angle = Mathf.Atan2(offsetY, offsetX);
        float rough = 1f + 0.12f * Mathf.Sin(5f * angle + 1f) + 0.08f * Mathf.Sin(9f * angle);
        float distance = Mathf.Sqrt(offsetX * offsetX + offsetY * offsetY) / rough;
        return 1f - ProceduralSprite.SoftEdge(0.55f, 0.95f, distance);
    }

    private static float GlowAlpha(float offsetX, float offsetY)
    {
        float distance = Mathf.Sqrt(offsetX * offsetX + offsetY * offsetY);
        return 1f - ProceduralSprite.SoftEdge(0f, 0.9f, distance);
    }

    private static float RingAlpha(float offsetX, float offsetY)
    {
        float distance = Mathf.Sqrt(offsetX * offsetX + offsetY * offsetY);
        return 1f - ProceduralSprite.SoftEdge(0f, 0.08f, Mathf.Abs(distance - 0.85f));
    }

    private class Scar
    {
        public SpriteRenderer Mark;
        public SpriteRenderer Glow;
        public SpriteRenderer Ring;
        public Coroutine Routine;
    }
}
