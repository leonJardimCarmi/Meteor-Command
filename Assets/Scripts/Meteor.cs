using UnityEngine;

public enum MeteorType
{
    Small,
    Large,
    Scout
}

public class Meteor : MonoBehaviour
{
    public static event System.Action<Vector3, float> Destroyed;
    public static event System.Action<Vector3, float> Impacted;

    [SerializeField] private float _largeScale = 1f;
    [SerializeField] private float _smallScale = 0.5f;
    [SerializeField] private float _fragmentSpreadAngle = 25f;

    [Header("Scout")]
    [SerializeField] private float _scoutScale = 1f;
    [SerializeField] private float _scoutSpeedMultiplier = 2f;
    [SerializeField] private Material _scoutMaterial;
    [SerializeField] private Color _scoutTrailColor = new Color(0.7f, 0.3f, 1f);

    [Header("Trail")]
    [SerializeField] private float _trailTime = 0.55f;
    [SerializeField] private float _trailWidthPerSize = 0.2f;
    [SerializeField] private Material _smokeMaterial;

    private MeshRenderer _renderer;
    private TrailRenderer _trail;
    private Material _normalMaterial;
    private Gradient _normalTrail;
    private Gradient _scoutTrail;
    private ParticleSystem _smoke;

    private Vector3 _direction;
    private float _speed;
    private MeteorType _type;
    private int _immuneBlastId;

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _trail = GetComponent<TrailRenderer>();
        _normalMaterial = _renderer.sharedMaterial;
        _normalTrail = TrailStyle.Flame();
        _scoutTrail = TrailStyle.Glow(_scoutTrailColor);

        if (_smokeMaterial != null)
        {
            _smoke = SmokeTrail.Create(transform, _smokeMaterial);
        }
    }

    public void Launch(Vector3 direction, float speed, MeteorType type, int immuneBlastId = 0)
    {
        _direction = direction.normalized;
        _type = type;
        _speed = type == MeteorType.Scout ? speed * _scoutSpeedMultiplier : speed;
        _immuneBlastId = immuneBlastId;
        transform.localScale = Vector3.one * ScaleOf(type);
        ApplyLook(type);
    }

    public bool Kill(int blastId)
    {
        if (!isActiveAndEnabled || blastId == _immuneBlastId)
        {
            return false;
        }

        if (_type == MeteorType.Large)
        {
            SpawnFragment(_fragmentSpreadAngle, blastId);
            SpawnFragment(-_fragmentSpreadAngle, blastId);
        }

        PoolManager.Instance.Release(gameObject);
        Destroyed?.Invoke(transform.position, transform.localScale.x);
        return true;
    }

    private void Update()
    {
        Move();

        if (HasReachedGround())
        {
            HitGround();
        }
    }

    private float ScaleOf(MeteorType type)
    {
        return type switch
        {
            MeteorType.Large => _largeScale,
            MeteorType.Scout => _scoutScale,
            _ => _smallScale
        };
    }

    // A scout gets its own material and trail colour, so it is recognised at a glance.
    private void ApplyLook(MeteorType type)
    {
        bool isScout = type == MeteorType.Scout;

        _renderer.sharedMaterial = isScout && _scoutMaterial != null ? _scoutMaterial : _normalMaterial;
        TrailStyle.Apply(_trail, _trailTime, _trailWidthPerSize * transform.localScale.x, isScout ? _scoutTrail : _normalTrail);

        if (_smoke != null)
        {
            SmokeTrail.Restart(_smoke, transform.localScale.x);
        }
    }

    private void Move()
    {
        transform.position += _direction * (_speed * Time.deltaTime);
    }

    private bool HasReachedGround()
    {
        float radius = transform.localScale.x * 0.5f;
        return transform.position.y - radius <= 0f;
    }

    private void HitGround()
    {
        CityManager.Instance.HitAt(transform.position);
        PoolManager.Instance.Release(gameObject);
        Impacted?.Invoke(transform.position, transform.localScale.x);
    }

    private void SpawnFragment(float angle, int blastId)
    {
        Vector3 direction = Quaternion.Euler(0f, 0f, angle) * _direction;
        GameObject fragment = PoolManager.Instance.Get(PoolType.Meteor, transform.position, Quaternion.identity);
        fragment.GetComponent<Meteor>().Launch(direction, _speed, MeteorType.Small, blastId);
    }
}
