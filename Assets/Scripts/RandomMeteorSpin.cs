using UnityEngine;

// Tumbles a rock in the air. The rock turns around its own centre, not around the pivot of the model (which for a
// rock sits at its base), so the centre of the rock stays exactly on the meteor and the fire trail always starts
// from the middle of it, whatever way the rock is turned.
[RequireComponent(typeof(MeshFilter))]
public class RandomMeteorSpin : MonoBehaviour
{
    [SerializeField] private float _minTumbleSpeed = 20f;
    [SerializeField] private float _maxTumbleSpeed = 60f;

    private Vector3 _meshCenter;
    private Vector3 _tumbleAxis;
    private float _tumbleSpeed;
    private Quaternion _rotation;

    private void Awake()
    {
        _meshCenter = GetComponent<MeshFilter>().sharedMesh.bounds.center;
    }

    private void OnEnable()
    {
        _rotation = Random.rotation;
        _tumbleAxis = Random.onUnitSphere;
        _tumbleSpeed = Random.Range(_minTumbleSpeed, _maxTumbleSpeed);
        ApplyPose();
    }

    private void Update()
    {
        _rotation = Quaternion.AngleAxis(_tumbleSpeed * Time.deltaTime, _tumbleAxis) * _rotation;
        ApplyPose();
    }

    // The offset moves the rock so that its own centre, not its pivot, is what stays on the parent.
    private void ApplyPose()
    {
        transform.localRotation = _rotation;
        transform.localPosition = -(_rotation * Vector3.Scale(_meshCenter, transform.localScale));
    }
}
