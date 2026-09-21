using UnityEngine;

public class Interceptor : MonoBehaviour
{
    public static event System.Action<Vector3> Arrived;

    [SerializeField] private float _speed = 30f;
    [SerializeField] private float _trailTime = 1.0f;
    [SerializeField] private float _trailWidth = 0.55f;

    private TrailRenderer _trail;
    private Gradient _trailColors;
    private Vector3 _target;

    private void Awake()
    {
        _trail = GetComponent<TrailRenderer>();
        _trailColors = TrailStyle.Smoke();
        GetComponent<MeshFilter>().sharedMesh = MissileMesh.Shared;
    }

    public void Launch(Vector3 target)
    {
        _target = target;
        FaceTarget();
        TrailStyle.Apply(_trail, _trailTime, _trailWidth, _trailColors);
    }

    private void Update()
    {
        MoveTowardTarget();

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

    private bool HasArrived()
    {
        return transform.position == _target;
    }

    private void Arrive()
    {
        PoolManager.Instance.Get(PoolType.Blast, _target, Quaternion.identity);
        PoolManager.Instance.Release(gameObject);
        Arrived?.Invoke(_target);
    }
}