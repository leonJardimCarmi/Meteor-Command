using UnityEngine;

public class CityManager : MonoBehaviour
{
    public static CityManager Instance { get; private set; }

    [SerializeField] private City[] _cities;
    [SerializeField] private float _hitRadius = 2f;

    private void Awake()
    {
        Instance = this;
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
            float distance = Vector3.Distance(city.transform.position, point);

            if (city.IsAlive && distance <= bestDistance)
            {
                target = city;
                bestDistance = distance;
            }
        }

        return target;
    }
}