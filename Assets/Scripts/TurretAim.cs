using UnityEngine;


[ExecuteAlways]
public class TurretAim : MonoBehaviour
{
    [SerializeField]
    private float maxYawAngle = 60f; 

    [SerializeField]
    private float minPitchAngle = 10f; 

    [SerializeField]
    private float maxPitchAngle = 80f; 

    [SerializeField]
    private float rotationSpeed = 10f; 

    private Camera _mainCamera;
    private static readonly Plane PlayPlane = new Plane(Vector3.forward, Vector3.zero);

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

        float clampedYaw = Mathf.Clamp(yaw, -maxYawAngle, maxYawAngle);
        float clampedPitch = Mathf.Clamp(pitch, minPitchAngle, maxPitchAngle);
        
        Quaternion targetLocalRotation = Quaternion.Euler(-clampedPitch, clampedYaw, 0f);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetLocalRotation, rotationSpeed * Time.deltaTime);
    }
}