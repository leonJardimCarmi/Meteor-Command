using UnityEngine;

// Places a flat quad with the sky image far behind the play area and sizes it so it always fills the
// camera view, whatever the screen shape. The camera never moves, so a still picture is enough.
public class BackgroundFit : MonoBehaviour
{
    [SerializeField] private float _distance = 60f;
    [SerializeField] private float _imageAspect = 1.7778f;
    [SerializeField] private float _coverMargin = 1.15f;
    [SerializeField] private float _verticalShift = 0.06f;

    private void Start()
    {
        RemoveCollider();
        DisableShadows();
        Fit(Camera.main);
    }

    // The background is only a picture: no physics, no shadows.
    private void RemoveCollider()
    {
        if (TryGetComponent(out Collider backgroundCollider))
        {
            Destroy(backgroundCollider);
        }
    }

    private void DisableShadows()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;
    }

    private void Fit(Camera camera)
    {
        float viewHeight = 2f * _distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float viewWidth = viewHeight * camera.aspect;

        float width = Mathf.Max(viewWidth, viewHeight * _imageAspect) * _coverMargin;
        float height = width / _imageAspect;

        Transform cameraTransform = camera.transform;
        transform.position = cameraTransform.position
            + cameraTransform.forward * _distance
            + cameraTransform.up * (viewHeight * _verticalShift);
        transform.rotation = cameraTransform.rotation;
        transform.localScale = new Vector3(width, height, 1f);
    }
}
