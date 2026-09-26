using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public static event System.Action<int> WaveStarted;
    public static event System.Action<int> WaveSizeDetermined;
    public static event System.Action WaveSpawning;
    public static event System.Action<float> ProgressChanged;

    [Header("Wave size")]
    [SerializeField] private int _meteorsBase = 6;
    [SerializeField] private int _meteorsPerWave = 3;
    [SerializeField] private float _spawnInterval = 1.5f;
    [SerializeField] private float _pauseBetweenWaves = 2f;

    [Header("Speed")]
    [SerializeField] private float _meteorSpeedBase = 6f;
    [SerializeField] private float _meteorSpeedPerWave = 0.5f;

    [Header("Large meteors")]
    [SerializeField] private int _firstLargeWave = 3;
    [SerializeField] [Range(0f, 1f)] private float _largeChanceBase = 0.25f;
    [SerializeField] [Range(0f, 1f)] private float _largeChancePerWave = 0.1f;
    [SerializeField] [Range(0f, 1f)] private float _largeChanceCap = 0.6f;

    [Header("Scout meteors")]
    [SerializeField] private int _firstScoutWave = 4;
    [SerializeField] [Range(0f, 1f)] private float _scoutChanceBase = 0.1f;
    [SerializeField] [Range(0f, 1f)] private float _scoutChancePerWave = 0.05f;
    [SerializeField] [Range(0f, 1f)] private float _scoutChanceCap = 0.3f;

    [Header("Area")]
    [SerializeField] private float _spawnHeight = 28f;
    [SerializeField] private float _spawnRangeX = 18f;
    [SerializeField] private float _targetRangeX = 16f;

    private int _wave;
    private int _spawnedThisWave;
    private bool _isWaveActive;

    private int MeteorCount => _meteorsBase + _meteorsPerWave * (_wave - 1);

    private float MeteorSpeed => _meteorSpeedBase + _meteorSpeedPerWave * (_wave - 1);

    private float LargeChance
    {
        get
        {
            if (_wave < _firstLargeWave)
            {
                return 0f;
            }

            float chance = _largeChanceBase + _largeChancePerWave * (_wave - _firstLargeWave);
            return Mathf.Min(chance, _largeChanceCap);
        }
    }

    private float ScoutChance
    {
        get
        {
            if (_wave < _firstScoutWave)
            {
                return 0f;
            }

            float chance = _scoutChanceBase + _scoutChancePerWave * (_wave - _firstScoutWave);
            return Mathf.Min(chance, _scoutChanceCap);
        }
    }

    private void OnEnable()
    {
        GameManager.GameStarted += BeginWaves;
        GameManager.GameOver += StopSpawning;
    }

    private void OnDisable()
    {
        GameManager.GameStarted -= BeginWaves;
        GameManager.GameOver -= StopSpawning;
    }

    private void Update()
    {
        if (_isWaveActive)
        {
            ProgressChanged?.Invoke(RemainingFraction());
        }
    }

    private void BeginWaves()
    {
        StartCoroutine(RunWaves());
    }

    private IEnumerator RunWaves()
    {
        while (true)
        {
            _wave++;
            _spawnedThisWave = 0;
            _isWaveActive = true;
            WaveStarted?.Invoke(_wave);
            WaveSizeDetermined?.Invoke(MeteorCount);
            yield return new WaitForSeconds(_pauseBetweenWaves);
            WaveSpawning?.Invoke();
            yield return StartCoroutine(SpawnWave());
            yield return new WaitUntil(IsSkyEmpty);
            _isWaveActive = false;
            ProgressChanged?.Invoke(0f);
            GameManager.Instance.CompleteWave();
        }
    }

    private IEnumerator SpawnWave()
    {
        WaitForSeconds delay = new WaitForSeconds(_spawnInterval);

        for (int i = 0; i < MeteorCount; i++)
        {
            SpawnMeteor();
            yield return delay;
        }
    }

    private bool IsSkyEmpty()
    {
        return PoolManager.Instance.CountActive(PoolType.Meteor) == 0;
    }

    // The share of the wave still to deal with: meteors not yet spawned plus meteors still in the sky.
    private float RemainingFraction()
    {
        int notSpawned = MeteorCount - _spawnedThisWave;
        int inSky = PoolManager.Instance.CountActive(PoolType.Meteor);
        return Mathf.Clamp01((float)(notSpawned + inSky) / MeteorCount);
    }

    private void SpawnMeteor()
    {
        _spawnedThisWave++;
        Vector3 start = new Vector3(Random.Range(-_spawnRangeX, _spawnRangeX), _spawnHeight, 0f);
        Vector3 target = new Vector3(Random.Range(-_targetRangeX, _targetRangeX), 0f, 0f);

        GameObject meteor = PoolManager.Instance.Get(PoolType.Meteor, start, Quaternion.identity);
        meteor.GetComponent<Meteor>().Launch(target - start, MeteorSpeed, PickType());
    }

    private MeteorType PickType()
    {
        float roll = Random.value;

        if (roll < ScoutChance)
        {
            return MeteorType.Scout;
        }

        return roll < ScoutChance + LargeChance ? MeteorType.Large : MeteorType.Small;
    }

    private void StopSpawning()
    {
        StopAllCoroutines();
    }
}