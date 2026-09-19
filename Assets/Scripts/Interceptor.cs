using UnityEngine;

public class Interceptor : MonoBehaviour
{
    [SerializeField] private float _speed = 30f;

    private Vector3 _target;

    public void Launch(Vector3 target)
    {
        _target = target;
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
        PoolManager.Instance.Release(gameObject);
    }
}