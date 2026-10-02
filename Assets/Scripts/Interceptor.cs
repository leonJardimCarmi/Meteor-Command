using UnityEngine;

public class Interceptor : MonoBehaviour
{
    public static event System.Action<Vector3> Arrived;

    [SerializeField] private float _speed = 30f;

    [Header("Exhaust and smoke")]
    [SerializeField] private float _trailTime = 0.3f;
    [SerializeField] private float _trailWidth = 0.3f;
    [SerializeField] private Material _smokeMaterial;

    // The smoke hangs a little behind the missile, so it never covers the meteors it is flying toward.
    private static readonly Vector3 SmokeOffset = new Vector3(0f, 0f, 0.8f);

    private TrailRenderer _trail;
    private Gradient _trailColors;
    private ParticleSystem _smoke;
    private TargetMarker _marker;
    private Vector3 _target;

    private void Awake()
    {
        _trail = GetComponent<TrailRenderer>();
        _trailColors = TrailStyle.Flame();

        // The smoke and the marker are not children of the missile: the smoke stays in the sky after the missile
        // is gone, and the marker is hidden by the missile itself when it arrives.
        if (_smokeMaterial != null)
        {
            _smoke = MissileSmoke.Create(transform.parent, _smokeMaterial);
        }

        _marker = TargetMarker.Create(transform.parent, _smokeMaterial);
    }

    public void Launch(Vector3 target)
    {
        _target = target;
        FaceTarget();
        TrailStyle.Apply(_trail, _trailTime, _trailWidth, _trailColors);

        _marker.Show(target);

        if (_smoke != null)
        {
            MissileSmoke.Begin(_smoke, transform.position + SmokeOffset);
        }
    }

    private void Update()
    {
        MoveTowardTarget();
        FollowWithSmoke();

        if (HasArrived())
        {
            Arrive();
        }
    }

    // The missile flies nose first toward the point it was fired at.
    private void FaceTarget()
    {
        Vector3 direction = _target - transform.position;

        if (direction.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void MoveTowardTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, _target, _speed * Time.deltaTime);
    }

    private void FollowWithSmoke()
    {
        if (_smoke != null)
        {
            _smoke.transform.position = transform.position + SmokeOffset;
        }
    }

    private bool HasArrived()
    {
        return transform.position == _target;
    }

    private void Arrive()
    {
        _marker.Hide();

        if (_smoke != null)
        {
            MissileSmoke.End(_smoke);
        }

        PoolManager.Instance.Get(PoolType.Blast, _target, Quaternion.identity);
        PoolManager.Instance.Release(gameObject);
        Arrived?.Invoke(_target);
    }
}
