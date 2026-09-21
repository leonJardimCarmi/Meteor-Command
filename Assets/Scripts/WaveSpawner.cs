using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public static event System.Action<int> WaveStarted;
    public static event System.Action WaveSpawning;

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

    [Header("Area")]
    [SerializeField] private float _spawnHeight = 28f;
    [SerializeField] private float _spawnRangeX = 18f;
    [SerializeField] private float _targetRangeX = 16f;

    private int _wave;

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

    private void BeginWaves()
    {
        StartCoroutine(RunWaves());
    }

    private IEnumerator RunWaves()
    {
        while (true)
        {
            _wave++;
            WaveStarted?.Invoke(_wave);
            yield return new WaitForSeconds(_pauseBetweenWaves);
            WaveSpawning?.Invoke();
            yield return StartCoroutine(SpawnWave());
            yield return new WaitUntil(IsSkyEmpty);
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

    private void SpawnMeteor()
    {
        Vector3 start = new Vector3(Random.Range(-_spawnRangeX, _spawnRangeX), _spawnHeight, 0f);
        Vector3 target = new Vector3(Random.Range(-_targetRangeX, _targetRangeX), 0f, 0f);
        bool isLarge = Random.value < LargeChance;

        GameObject meteor = PoolManager.Instance.Get(PoolType.Meteor, start, Quaternion.identity);
        meteor.GetComponent<Meteor>().Launch(target - start, MeteorSpeed, isLarge);
    }

    private void StopSpawning()
    {
        StopAllCoroutines();
    }
}