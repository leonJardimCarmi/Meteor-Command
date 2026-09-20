using System;
using UnityEngine;

public class Battery : MonoBehaviour
{
    public static event Action<int> AmmoChanged;

    [SerializeField] private Vector3 _launchOffset = new Vector3(0f, 1.5f, 0f);

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
        return true;
    }

    private void LaunchInterceptor(Vector3 target)
    {
        Vector3 launchPosition = transform.position + _launchOffset;
        GameObject missile = PoolManager.Instance.Get(PoolType.Interceptor, launchPosition, Quaternion.identity);
        missile.GetComponent<Interceptor>().Launch(target);
    }
}