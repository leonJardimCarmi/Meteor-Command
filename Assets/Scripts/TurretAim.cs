using UnityEngine;

// Turns the turret toward the mouse, within a limited sideways and upward angle.
[ExecuteAlways]
public class TurretAim : MonoBehaviour
{
    private static readonly Plane PlayPlane = new Plane(Vector3.forward, Vector3.zero);

    [SerializeField] private float _maxYawAngle = 60f;
    [SerializeField] private float _minPitchAngle = 10f;
    [SerializeField] private float _maxPitchAngle = 80f;
    [SerializeField] private float _rotationSpeed = 10f;

    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (!PlayPlane.Raycast(ray, out float distance))
        {
            return;
        }

        Vector3 targetPoint = ray.GetPoint(distance);
        Vector3 toTarget = targetPoint - transform.position;

        float horizontalDistance = new Vector2(toTarget.x, toTarget.z).magnitude;
        float yaw = Vector3.SignedAngle(Vector3.forward, new Vector3(toTarget.x, 0f, toTarget.z), Vector3.up);
        float pitch = Mathf.Atan2(toTarget.y, horizontalDistance) * Mathf.Rad2Deg;

        float clampedYaw = Mathf.Clamp(yaw, -_maxYawAngle, _maxYawAngle);
        float clampedPitch = Mathf.Clamp(pitch, _minPitchAngle, _maxPitchAngle);

        Quaternion targetLocalRotation = Quaternion.Euler(-clampedPitch, clampedYaw, 0f);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetLocalRotation, _rotationSpeed * Time.deltaTime);
    }
}
