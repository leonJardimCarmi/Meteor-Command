using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    private Camera _camera;
    private Plane _playPlane;

    private void Awake()
    {
        _camera = Camera.main;
        _playPlane = new Plane(Vector3.forward, Vector3.zero);
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
            Debug.Log("Aim point: " + point);
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