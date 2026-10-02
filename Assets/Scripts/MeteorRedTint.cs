using UnityEngine;

// Gives the rock a faint red glow by tinting its colour, without making a new material for every meteor.
[RequireComponent(typeof(MeshRenderer))]
public class MeteorRedTint : MonoBehaviour
{
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

    [SerializeField] [Range(0f, 1f)] private float _redAmount = 0.3f;

    private MeshRenderer _meshRenderer;
    private MaterialPropertyBlock _propertyBlock;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _propertyBlock = new MaterialPropertyBlock();
    }

    private void OnEnable()
    {
        ApplyTint();
    }

    private void ApplyTint()
    {
        Color baseColor = _meshRenderer.sharedMaterial.GetColor(BaseColorId);
        Color tinted = Color.Lerp(baseColor, Color.red, _redAmount);

        _meshRenderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetColor(BaseColorId, tinted);
        _meshRenderer.SetPropertyBlock(_propertyBlock);
    }

#if UNITY_EDITOR
    // Lets the tint be seen while the amount is being adjusted in the Inspector.
    private void OnValidate()
    {
        if (_meshRenderer == null)
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        _propertyBlock ??= new MaterialPropertyBlock();
        ApplyTint();
    }
#endif
}
