using UnityEngine;


[RequireComponent(typeof(MeshRenderer))]
public class MeteorRedTint : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)]
    private float redAmount = 0.3f; 

    private MeshRenderer _meshRenderer;
    private MaterialPropertyBlock _propertyBlock;
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

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
        Color tinted = Color.Lerp(baseColor, Color.red, redAmount);

        _meshRenderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetColor(BaseColorId, tinted);
        _meshRenderer.SetPropertyBlock(_propertyBlock);
    }

#if UNITY_EDITOR
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