using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private readonly GameObject _prefab;
    private readonly Transform _parent;
    private readonly Queue<GameObject> _available = new Queue<GameObject>();
    private readonly HashSet<GameObject> _all = new HashSet<GameObject>();
    
    public int ActiveCount => _all.Count - _available.Count;

    public ObjectPool(GameObject prefab, int initialCount, Transform parent)
    {
        _prefab = prefab;
        _parent = parent;

        for (int i = 0; i < initialCount; i++)
        {
            _available.Enqueue(CreateInstance());
        }
    }

    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        GameObject instance = _available.Count > 0 ? _available.Dequeue() : CreateInstance();

        instance.transform.SetPositionAndRotation(position, rotation);
        instance.SetActive(true);
        return instance;
    }

    public void Release(GameObject instance)
    {
        if (!instance.activeSelf)
        {
            return;
        }

        instance.SetActive(false);
        _available.Enqueue(instance);
    }

    public bool Owns(GameObject instance)
    {
        return _all.Contains(instance);
    }

    private GameObject CreateInstance()
    {
        GameObject instance = Object.Instantiate(_prefab, _parent);
        instance.SetActive(false);
        _all.Add(instance);
        return instance;
    }
}