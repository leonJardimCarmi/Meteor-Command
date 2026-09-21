using UnityEngine;

public class CityManager : MonoBehaviour
{
    public static CityManager Instance { get; private set; }

    [SerializeField] private City[] _cities;
    [SerializeField] private float _hitRadius = 2f;

    public int CityCount => _cities.Length;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < _cities.Length; i++)
        {
            _cities[i].SetStyle(i);
        }
    }

    public bool IsCityAlive(int index)
    {
        return _cities[index].IsAlive;
    }

    public int CountAlive()
    {
        int alive = 0;

        foreach (City city in _cities)
        {
            if (city.IsAlive)
            {
                alive++;
            }
        }

        return alive;
    }

    public void HitAt(Vector3 point)
    {
        City target = FindTarget(point);

        if (target != null)
        {
            target.Collapse();
        }
    }

    private City FindTarget(Vector3 point)
    {
        City target = null;
        float bestDistance = _hitRadius;

        foreach (City city in _cities)
        {
            float distance = Mathf.Abs(city.transform.position.x - point.x);

            if (city.IsAlive && distance <= bestDistance)
            {
                target = city;
                bestDistance = distance;
            }
        }

        return target;
    }
}
