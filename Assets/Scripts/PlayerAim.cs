using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] private Battery _battery;

    private readonly List<RaycastResult> _hits = new List<RaycastResult>();
    private Camera _camera;
    private Plane _playPlane;
    private bool _isWaveIntro;

    private void Awake()
    {
        _camera = Camera.main;
        _playPlane = new Plane(Vector3.forward, Vector3.zero);
    }

    private void OnEnable()
    {
        WaveSpawner.WaveStarted += OnWaveStarted;
        WaveSpawner.WaveSpawning += OnWaveSpawning;
    }

    private void OnDisable()
    {
        WaveSpawner.WaveStarted -= OnWaveStarted;
        WaveSpawner.WaveSpawning -= OnWaveSpawning;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && CanFire())
        {
            HandleClick();
        }
    }

    // Shots are ignored while the wave banner is up, so no ammo is wasted before the sky is visible.
    private void OnWaveStarted(int wave)
    {
        _isWaveIntro = true;
    }

    private void OnWaveSpawning()
    {
        _isWaveIntro = false;
    }

    private bool CanFire()
    {
        return GameManager.Instance.IsPlaying && !_isWaveIntro && !IsPointerOverButton();
    }

    // Only buttons swallow a click, so pressing Pause cannot also spend a shot. The HUD text sits under
    // the pointer at times too, and it must never block a shot.
    private bool IsPointerOverButton()
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        EventSystem.current.RaycastAll(pointer, _hits);

        foreach (RaycastResult hit in _hits)
        {
            if (hit.gameObject.GetComponentInParent<Selectable>() != null)
            {
                return true;
            }
        }

        return false;
    }

    private void HandleClick()
    {
        if (TryGetPlanePoint(out Vector3 point))
        {
            _battery.TryFire(point);
        }
    }

    private bool TryGetPlanePoint(out Vector3 point)
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (_playPlane.Raycast(ray, out float distance))
        {
            point = ray.GetPoint(distance);
            return true;
        }

        point = Vector3.zero;
        return false;
    }
}
