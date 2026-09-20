using UnityEngine;

public class Meteor : MonoBehaviour
{
    [SerializeField] private float _largeScale = 1f;
    [SerializeField] private float _smallScale = 0.5f;
    [SerializeField] private float _fragmentSpreadAngle = 25f;

    private Vector3 _direction;
    private float _speed;
    private bool _isLarge;
    private int _immuneBlastId;

    public void Launch(Vector3 direction, float speed, bool isLarge, int immuneBlastId = 0)
    {
        _direction = direction.normalized;
        _speed = speed;
        _isLarge = isLarge;
        _immuneBlastId = immuneBlastId;
        transform.localScale = Vector3.one * (isLarge ? _largeScale : _smallScale);
    }

    public bool Kill(int blastId)
    {
        if (!isActiveAndEnabled || blastId == _immuneBlastId)
        {
            return false;
        }

        if (_isLarge)
        {
            SpawnFragment(_fragmentSpreadAngle, blastId);
            SpawnFragment(-_fragmentSpreadAngle, blastId);
        }

        PoolManager.Instance.Release(gameObject);
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
    }

    private void SpawnFragment(float angle, int blastId)
    {
        Vector3 direction = Quaternion.Euler(0f, 0f, angle) * _direction;
        GameObject fragment = PoolManager.Instance.Get(PoolType.Meteor, transform.position, Quaternion.identity);
        fragment.GetComponent<Meteor>().Launch(direction, _speed, false, blastId);
    }
}