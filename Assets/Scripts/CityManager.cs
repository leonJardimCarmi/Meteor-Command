using UnityEngine;

public class CityManager : MonoBehaviour
{
    public static CityManager Instance { get; private set; }

    [SerializeField] private City[] _cities;
    [SerializeField] private float _hitRadius = 2f;

    public int CityCount => _cities.Length;

    // Shows how close to each city a meteor has to land to hit it, in the Scene view.
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.4f, 0.9f, 1f);

        foreach (City city in _cities)
        {
            Vector3 ground = new Vector3(city.transform.position.x, 0f, city.transform.position.z);
            Gizmos.DrawLine(ground + Vector3.left * _hitRadius, ground + Vector3.right * _hitRadius);
        }
    }

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
