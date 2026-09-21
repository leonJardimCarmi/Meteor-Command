using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Shows one small icon per city on the HUD. An icon turns dark when its city is destroyed.
// The scene holds a single template icon; the others are copies of it, laid out by a layout group.
public class CityIcons : MonoBehaviour
{
    [SerializeField] private Image _template;
    [SerializeField] private Color _aliveColor = new Color(1f, 0.8f, 0.37f);
    [SerializeField] private Color _destroyedColor = new Color(0.25f, 0.27f, 0.33f);

    private readonly List<Image> _icons = new List<Image>();

    private void OnEnable()
    {
        City.Destroyed += Refresh;
    }

    private void OnDisable()
    {
        City.Destroyed -= Refresh;
    }

    private void Start()
    {
        CreateIcons();
        Refresh();
    }

    private void CreateIcons()
    {
        _icons.Add(_template);

        for (int i = 1; i < CityManager.Instance.CityCount; i++)
        {
            _icons.Add(Instantiate(_template, transform));
        }
    }

    private void Refresh()
    {
        for (int i = 0; i < _icons.Count; i++)
        {
            _icons[i].color = CityManager.Instance.IsCityAlive(i) ? _aliveColor : _destroyedColor;
        }
    }
}
