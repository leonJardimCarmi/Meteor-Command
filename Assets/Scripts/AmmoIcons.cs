using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// A row of small missiles, one per shot in a wave. Spent shots fade out from the right.
// The scene holds a single template icon; the others are copies of it, laid out by a layout group.
public class AmmoIcons : MonoBehaviour
{
    [SerializeField] private Image _template;
    [SerializeField] private Color _readyColor = new Color(0.6f, 0.85f, 1f);
    [SerializeField] private Color _spentColor = new Color(0.25f, 0.3f, 0.4f, 0.6f);

    private readonly List<Image> _icons = new List<Image>();
    private int _ammo;

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

        CreateIcons();
        Refresh(_ammo);
    }

    private void CreateIcons()
    {
        _icons.Add(_template);

        for (int i = 1; i < GameManager.Instance.AmmoPerWave; i++)
        {
            _icons.Add(Instantiate(_template, transform));
        }
    }

    private void Refresh(int ammo)
    {
        _ammo = ammo;

        for (int i = 0; i < _icons.Count; i++)
        {
            _icons[i].color = i < ammo ? _readyColor : _spentColor;
        }
    }
}
