using System;
using UnityEngine;

public class Battery : MonoBehaviour
{
    public static event Action<int> AmmoChanged;
    public static event Action Fired;

    [SerializeField] private Transform _muzzle;

    public int Ammo { get; private set; }

    public void Refill(int amount)
    {
        Ammo = amount;
        AmmoChanged?.Invoke(Ammo);
    }

    public bool TryFire(Vector3 target)
    {
        if (Ammo <= 0)
        {
            return false;
        }

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