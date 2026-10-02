using System;
using UnityEngine;

[RequireComponent(typeof(TurretHealth))]
public class Battery : MonoBehaviour
{
    public static event Action<int> AmmoChanged;
    public static event Action Fired;

    [SerializeField] private Transform _muzzle;
    [SerializeField] private float _reloadSeconds = 0.5f;

    private TurretHealth _health;
    private float _nextShotTime;

    public int Ammo { get; private set; }

    private void Awake()
    {
        _health = GetComponent<TurretHealth>();
    }

    public void Refill(int amount)
    {
        Ammo = amount;
        AmmoChanged?.Invoke(Ammo);
    }

    public void Repair()
    {
        _health.Repair();
    }

    // A shot is refused while the battery is still reloading from the one before. Every hit the turret has taken
    // makes that reload take longer, and a destroyed turret cannot fire at all.
    public bool TryFire(Vector3 target)
    {
        if (Ammo <= 0 || _health.IsDestroyed || Time.time < _nextShotTime)
        {
            return false;
        }

        _nextShotTime = Time.time + _reloadSeconds * _health.ReloadMultiplier;
        Ammo--;
        AmmoChanged?.Invoke(Ammo);
        LaunchInterceptor(target);
        Fired?.Invoke();
        return true;
    }

    private void LaunchInterceptor(Vector3 target)
    {
        GameObject missile = PoolManager.Instance.Get(PoolType.Interceptor, _muzzle.position, Quaternion.identity);
        missile.GetComponent<Interceptor>().Launch(target);
    }
}
