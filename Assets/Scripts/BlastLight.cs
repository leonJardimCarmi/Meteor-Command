using UnityEngine;

// A point light that follows a blast and grows and fades with its size, so the explosion lights up
// the cities and the ground around it. The light is created in code and sits a little in front of the
// play plane, otherwise it would be inside the buildings and light nothing.
[RequireComponent(typeof(Blast))]
public class BlastLight : MonoBehaviour
{
    [SerializeField] private Color _color = new Color(0.3f, 0.9f, 1f);
    [SerializeField] private float _maxIntensity = 6f;
    [SerializeField] private float _range = 14f;
    [SerializeField] private float _distanceInFront = 4f;

    private Light _light;
    private float _fullSize;

    private void Awake()
    {
        _light = CreateLight();
        _fullSize = GetComponent<Blast>().Radius * 2f;
    }

    private void Update()
    {
        _light.transform.position = transform.position + Vector3.back * _distanceInFront;
        _light.intensity = _maxIntensity * Mathf.Clamp01(transform.localScale.x / _fullSize);
    }

    private Light CreateLight()
    {
        GameObject lightObject = new GameObject("Blast Light");
        lightObject.transform.SetParent(transform, false);

        Light blastLight = lightObject.AddComponent<Light>();
        blastLight.type = LightType.Point;
        blastLight.color = _color;
        blastLight.range = _range;
        blastLight.shadows = LightShadows.None;
        return blastLight;
    }
}
