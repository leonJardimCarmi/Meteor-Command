using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// A row of small round lights that show how many hits the turret can still take. They are made in code, one
// for each hit point, and turn red on the last one.
public class TurretHealthIcons : MonoBehaviour
{
    private const int TextureSize = 64;

    [SerializeField] private float _iconSize = 30f;
    [SerializeField] private float _spacing = 10f;
    [SerializeField] private Color _healthyColor = new Color(0.55f, 0.95f, 1f);
    [SerializeField] private Color _lastHitColor = new Color(1f, 0.3f, 0.25f);
    [SerializeField] private Color _lostColor = new Color(1f, 1f, 1f, 0.15f);

    private readonly List<Image> _icons = new List<Image>();
    private Sprite _sprite;

    private void Awake()
    {
        _sprite = ProceduralSprite.Create(TextureSize, DiscAlpha);
    }

    private void OnEnable()
    {
        TurretHealth.HitPointsChanged += Refresh;
    }

    private void OnDisable()
    {
        TurretHealth.HitPointsChanged -= Refresh;
    }

    private void Refresh(int hitPoints, int maxHitPoints)
    {
        GrowTo(maxHitPoints);

        for (int i = 0; i < _icons.Count; i++)
        {
            _icons[i].color = i < hitPoints ? (hitPoints == 1 ? _lastHitColor : _healthyColor) : _lostColor;
        }
    }

    private void GrowTo(int count)
    {
        while (_icons.Count < count)
        {
            _icons.Add(CreateIcon());
        }

        for (int i = 0; i < _icons.Count; i++)
        {
            float centred = i - (_icons.Count - 1) * 0.5f;
            ((RectTransform)_icons[i].transform).anchoredPosition = new Vector2(centred * (_iconSize + _spacing), 0f);
        }
    }

    private Image CreateIcon()
    {
        GameObject iconObject = new GameObject("Turret Hit Point", typeof(RectTransform), typeof(Image));
        iconObject.transform.SetParent(transform, false);

        Image icon = iconObject.GetComponent<Image>();
        icon.sprite = _sprite;
        icon.raycastTarget = false;
        ((RectTransform)iconObject.transform).sizeDelta = new Vector2(_iconSize, _iconSize);
        return icon;
    }

    // A solid disc with a soft edge.
    private static float DiscAlpha(float offsetX, float offsetY)
    {
        float distance = Mathf.Sqrt(offsetX * offsetX + offsetY * offsetY);
        return 1f - ProceduralSprite.SoftEdge(0.75f, 0.95f, distance);
    }
}
