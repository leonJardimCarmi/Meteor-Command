using UnityEngine;

public class Interceptor : MonoBehaviour
{
    public static event System.Action<Vector3> Arrived;

    [SerializeField] private float _speed = 30f;
    [SerializeField] private float _trailTime = 0.8f;
    [SerializeField] private float _trailWidth = 0.6f;

    private TrailRenderer _trail;
    private Vector3 _target;

    private void Awake()
    {
        _trail = GetComponent<TrailRenderer>();
    }

    public void Launch(Vector3 target)
    {
        _target = target;
        TrailStyle.Apply(_trail, _trailTime, _trailWidth, TrailStyle.Smoke());
    }

    private void Update()
    {
        MoveTowardTarget();

        if (HasArrived())
        {
            Arrive();
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