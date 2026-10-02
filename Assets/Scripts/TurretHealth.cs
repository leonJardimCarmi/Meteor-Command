using System;
using UnityEngine;

// The turret can take a few hits. Every hit doubles how long it takes to reload, and the last one destroys it.
// A meteor that lands close enough to the turret counts as a hit, the same way it does for a city. Repairs give
// back one hit point at a time.
public class TurretHealth : MonoBehaviour
{
    public static event Action Hit;
    public static event Action Destroyed;
    public static event Action<int, int> HitPointsChanged;

    [SerializeField] private int _maxHitPoints = 3;
    [SerializeField] private float _hitRadius = 1.5f;

    public int HitPoints { get; private set; }
    public bool IsDestroyed => HitPoints <= 0;

    // 1 at full health, then 2 and 4 after the first and second hit: the reload time is multiplied by this.
    public float ReloadMultiplier => Mathf.Pow(2f, _maxHitPoints - HitPoints);

    private void Awake()
    {
        HitPoints = _maxHitPoints;
    }

    private void OnEnable()
    {
        Meteor.Impacted += OnMeteorImpacted;
    }

    private void OnDisable()
    {
        Meteor.Impacted -= OnMeteorImpacted;
    }

    private void Start()
    {
        HitPointsChanged?.Invoke(HitPoints, _maxHitPoints);
    }

    // Shows how close to the turret a meteor has to land to hit it, in the Scene view.
    private void OnDrawGizmos()
    {
        Vector3 ground = new Vector3(transform.position.x, 0f, transform.position.z);

        Gizmos.color = new Color(0.4f, 0.9f, 1f);
        Gizmos.DrawLine(ground + Vector3.left * _hitRadius, ground + Vector3.right * _hitRadius);
    }

    public void Repair()
    {
        if (IsDestroyed || HitPoints >= _maxHitPoints)
        {
            return;
        }

        HitPoints++;
        HitPointsChanged?.Invoke(HitPoints, _maxHitPoints);
    }

    private void OnMeteorImpacted(Vector3 position, float size)
    {
        bool isClose = Mathf.Abs(position.x - transform.position.x) <= _hitRadius;

        if (isClose && !IsDestroyed && GameManager.Instance.IsPlaying)
        {
            TakeHit();
        }
    }

    private void TakeHit()
    {
        HitPoints--;
        Hit?.Invoke();
        HitPointsChanged?.Invoke(HitPoints, _maxHitPoints);

        if (IsDestroyed)
        {
            Destroyed?.Invoke();
        }
    }
}
