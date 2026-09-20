using System.Collections.Generic;
using UnityEngine;

public enum PoolType
{
    Meteor,
    Interceptor,
    Blast
}

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [SerializeField] private GameObject _meteorPrefab;
    [SerializeField] private GameObject _interceptorPrefab;
    [SerializeField] private GameObject _blastPrefab;
    [SerializeField] private int _meteorCount = 80;
    [SerializeField] private int _interceptorCount = 25;
    [SerializeField] private int _blastCount = 25;

    private readonly Dictionary<PoolType, ObjectPool> _pools = new Dictionary<PoolType, ObjectPool>();

    private void Awake()
    {
        Instance = this;
        CreatePools();
    }

    public GameObject Get(PoolType type, Vector3 position, Quaternion rotation)
    {
        return _pools[type].Get(position, rotation);
    }

    public void Release(GameObject instance)
    {
        foreach (ObjectPool pool in _pools.Values)
        {
            if (pool.Owns(instance))
            {
                pool.Release(instance);
                return;
            }
        }
    }
    
    public int CountActive(PoolType type)
    {
        return _pools[type].ActiveCount;
    }

    private void CreatePools()
    {
        _pools[PoolType.Meteor] = new ObjectPool(_meteorPrefab, _meteorCount, transform);
        _pools[PoolType.Interceptor] = new ObjectPool(_interceptorPrefab, _interceptorCount, transform);
        _pools[PoolType.Blast] = new ObjectPool(_blastPrefab, _blastCount, transform);
    }
}