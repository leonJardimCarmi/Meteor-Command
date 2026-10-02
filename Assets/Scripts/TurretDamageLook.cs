using UnityEngine;

// How the damage to the turret looks. One hit makes it smoke a little, two hits make it smoke heavily, and the
// last hit blows the gun apart and leaves a burning wreck. Repairs clear the smoke again.
// The smoke is not a child of the gun, because the gun turns and the smoke must always rise straight up.
public class TurretDamageLook : MonoBehaviour
{
    private const float LightSmokePerSecond = 3f;
    private const float HeavySmokePerSecond = 10f;
    private const float WreckBurnSeconds = 14f;

    private static readonly Vector3 SmokeOffset = new Vector3(0f, -0.3f, 0f);

    [SerializeField] private Material _smokeMaterial;
    [SerializeField] private Color _explosionColor = new Color(1f, 0.55f, 0.15f);
    [SerializeField] private float _explosionSize = 4f;

    private ParticleSystem _smoke;
    private RuinFire _fire;
    private Transform _smokeHolder;

    private void Awake()
    {
        _smokeHolder = new GameObject("Turret Smoke").transform;
        _smokeHolder.position = transform.position + SmokeOffset;
        _smoke = SmokePlume.Create(_smokeHolder, _smokeMaterial);
        _fire = RuinFire.Create(_smokeHolder);
    }

    private void OnEnable()
    {
        TurretHealth.HitPointsChanged += OnHitPointsChanged;
        TurretHealth.Destroyed += OnDestroyed;
    }

    private void OnDisable()
    {
        TurretHealth.HitPointsChanged -= OnHitPointsChanged;
        TurretHealth.Destroyed -= OnDestroyed;
    }

    private void OnHitPointsChanged(int hitPoints, int maxHitPoints)
    {
        int hitsTaken = maxHitPoints - hitPoints;

        if (hitPoints <= 0)
        {
            return;
        }

        if (hitsTaken == 0)
        {
            _smoke.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
        else
        {
            SmokeRate(hitsTaken == 1 ? LightSmokePerSecond : HeavySmokePerSecond);
        }
    }

    private void OnDestroyed()
    {
        Explode(_explosionSize);
        Explode(_explosionSize * 0.6f);

        foreach (MeshRenderer gun in GetComponentsInChildren<MeshRenderer>())
        {
            gun.enabled = false;
        }

        SmokeRate(HeavySmokePerSecond);
        _fire.Ignite(_smokeHolder.position, WreckBurnSeconds);
    }

    private void SmokeRate(float puffsPerSecond)
    {
        ParticleSystem.EmissionModule emission = _smoke.emission;
        emission.rateOverTime = puffsPerSecond;

        if (!_smoke.isPlaying)
        {
            _smoke.Play();
        }
    }

    private void Explode(float size)
    {
        if (!PoolManager.Instance.HasPool(PoolType.Effect))
        {
            return;
        }

        GameObject effect = PoolManager.Instance.Get(PoolType.Effect, transform.position, Quaternion.identity);
        effect.GetComponent<ExplosionEffect>().Play(_explosionColor, size, true);
    }
}
