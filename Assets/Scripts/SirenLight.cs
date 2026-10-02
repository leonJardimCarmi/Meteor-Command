using UnityEngine;

// A rotating red warning light next to the battery, like the beacon on a real air defence launcher. A red spot
// light turns around on top of a small post. The lens and a small glow flash brightest each time the beam points
// at the camera, which is what makes it look like a siren.
public class SirenLight : MonoBehaviour
{
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

    [SerializeField] private Transform _spinner;
    [SerializeField] private Renderer _lens;
    [SerializeField] private Light _glow;
    [SerializeField] private float _degreesPerSecond = 300f;
    [SerializeField] private Color _color = new Color(1f, 0.05f, 0.03f);
    [SerializeField] private float _idleBrightness = 0.5f;
    [SerializeField] private float _flashBrightness = 5f;
    [SerializeField] private float _idleGlow = 0.3f;
    [SerializeField] private float _flashGlow = 4f;

    private MaterialPropertyBlock _block;
    private Camera _camera;

    private void Awake()
    {
        _block = new MaterialPropertyBlock();
        _camera = Camera.main;
    }

    private void Update()
    {
        _spinner.Rotate(0f, _degreesPerSecond * Time.deltaTime, 0f, Space.Self);

        float flash = FlashAmount();
        SetLensBrightness(Mathf.Lerp(_idleBrightness, _flashBrightness, flash));
        _glow.intensity = Mathf.Lerp(_idleGlow, _flashGlow, flash);
    }

    // 1 when the beam points straight at the camera, falling off quickly to 0 as it turns away.
    private float FlashAmount()
    {
        Vector3 toCamera = (_camera.transform.position - _spinner.position).normalized;
        float facing = Mathf.Max(0f, Vector3.Dot(_spinner.forward, toCamera));
        return facing * facing * facing;
    }

    private void SetLensBrightness(float brightness)
    {
        _lens.GetPropertyBlock(_block);
        _block.SetColor(EmissionColorId, _color * brightness);
        _lens.SetPropertyBlock(_block);
    }
}
