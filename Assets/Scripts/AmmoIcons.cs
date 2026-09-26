using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// A row of small missiles, one per shot in a wave. Spent shots fade out from the right. Ammo per wave grows
// as the waves do (see GameManager), so past Max Icons the row stops growing and instead shows a full bar
// that only starts emptying once the ammo left drops below that cap - a wide but still full bar reads better
// than a hundred slivers. The scene holds a single template icon; the others are copies of it, laid out by a
// layout group.
public class AmmoIcons : MonoBehaviour
{
    [SerializeField] private Image _template;
    [SerializeField] private Color _readyColor = new Color(0.6f, 0.85f, 1f);
    [SerializeField] private Color _spentColor = new Color(0.25f, 0.3f, 0.4f, 0.6f);
    [SerializeField] private int _maxIcons = 50;

    private readonly List<Image> _icons = new List<Image>();

    private void OnEnable()
    {
        Battery.AmmoChanged += Refresh;
    }

    private void OnDisable()
    {
        Battery.AmmoChanged -= Refresh;
    }

    private void Start()
    {
        // The template must be a child of this object. If the script sits on the template itself, every
        // copy would create more copies without end, so stop here instead.
        if (_template == null || _template.transform.parent != transform)
        {
            Debug.LogError("AmmoIcons must be on the parent object, with the icon as a child in the Template field.", this);
            enabled = false;
            return;
        }

        _icons.Add(_template);
    }

    private void Refresh(int ammo)
    {
        GrowTo(Mathf.Min(GameManager.Instance.AmmoPerWave, _maxIcons));
        int ready = Mathf.Min(ammo, _maxIcons);

        for (int i = 0; i < _icons.Count; i++)
        {
            _icons[i].color = i < ready ? _readyColor : _spentColor;
        }
    }

    // Adds icons until there are enough for the wave's ammo, capped at Max Icons. Waves never need fewer
    // than the one before, so icons are only ever added, never removed.
    private void GrowTo(int count)
    {
        while (_icons.Count < count)
        {
            _icons.Add(Instantiate(_template, transform));
        }
    }
}
