using UnityEngine;
using UnityEngine.UI;

// A crosshair that replaces the mouse arrow while a run is being played, so it is clear exactly where a shot
// will go. In the menus, on the pause screen and on game over the normal mouse arrow comes back.
// The crosshair is drawn in code, so there is no picture file to import.
[RequireComponent(typeof(Image))]
public class Crosshair : MonoBehaviour
{
    private const int TextureSize = 128;
    private const float RingRadius = 0.5f;
    private const float LineHalfWidth = 0.035f;

    [SerializeField] private float _size = 72f;
    [SerializeField] private Color _color = new Color(0.65f, 1f, 1f, 0.95f);

    private Image _image;
    private bool _wasPlaying;

    private void Awake()
    {
        // The crosshair moves every frame. On a canvas of its own, only the crosshair has to be redrawn for that,
        // not the whole HUD with it.
        gameObject.AddComponent<Canvas>();

        _image = GetComponent<Image>();
        _image.enabled = false;
        _image.sprite = ProceduralSprite.Create(TextureSize, CrosshairAlpha);
        _image.color = _color;
        _image.raycastTarget = false;
        ((RectTransform)transform).sizeDelta = new Vector2(_size, _size);
    }

    private void OnDisable()
    {
        Cursor.visible = true;
    }

    private void Update()
    {
        bool isPlaying = GameManager.Instance.IsPlaying;

        if (isPlaying != _wasPlaying)
        {
            _wasPlaying = isPlaying;
            _image.enabled = isPlaying;
            Cursor.visible = !isPlaying;
        }

        if (isPlaying)
        {
            transform.position = Input.mousePosition;
        }
    }

    // A thin ring, four short lines pointing at the middle from outside the ring, and a dot in the centre.
    private static float CrosshairAlpha(float offsetX, float offsetY)
    {
        float distance = Mathf.Sqrt(offsetX * offsetX + offsetY * offsetY);
        float ring = 1f - ProceduralSprite.SoftEdge(0f, 0.05f, Mathf.Abs(distance - RingRadius));
        float dot = 1f - ProceduralSprite.SoftEdge(0.03f, 0.07f, distance);

        float across = Mathf.Min(Mathf.Abs(offsetX), Mathf.Abs(offsetY));
        float along = Mathf.Max(Mathf.Abs(offsetX), Mathf.Abs(offsetY));
        float line = (1f - ProceduralSprite.SoftEdge(LineHalfWidth * 0.5f, LineHalfWidth * 1.5f, across)) * Mathf.Clamp01((along - 0.62f) * 40f) * (1f - ProceduralSprite.SoftEdge(0.9f, 0.95f, along));

        return Mathf.Clamp01(Mathf.Max(ring, dot, line));
    }
}
