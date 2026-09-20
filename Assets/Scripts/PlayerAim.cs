using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] private Battery _battery;

    private Camera _camera;
    private Plane _playPlane;

    private void Awake()
    {
        _camera = Camera.main;
        _playPlane = new Plane(Vector3.forward, Vector3.zero);
    }
    private void OnEnable()
    {
        GameManager.GameOver += DisableAiming;
    }

    private void OnDisable()
    {
        GameManager.GameOver -= DisableAiming;
    }

    private void DisableAiming()
    {
        enabled = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
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