using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private float _spawnHeight = 28f;
    [SerializeField] private float _spawnRangeX = 18f;
    [SerializeField] private float _targetRangeX = 16f;
    [SerializeField] private float _meteorSpeed = 6f;
    [SerializeField] private float _spawnInterval = 1.5f;
    [SerializeField] [Range(0f, 1f)] private float _largeChance = 0.4f;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }
    private void OnEnable()
    {
        GameManager.GameOver += StopSpawning;
    }

    private void OnDisable()
    {
        GameManager.GameOver -= StopSpawning;
    }

    private void StopSpawning()
    {
        StopAllCoroutines();
    }

    private IEnumerator SpawnLoop()
    {
        WaitForSeconds delay = new WaitForSeconds(_spawnInterval);

        while (true)
        {
            SpawnMeteor();
            yield return delay;
        }
    }

    private void SpawnMeteor()
    {
        Vector3 start = new Vector3(Random.Range(-_spawnRangeX, _spawnRangeX), _spawnHeight, 0f);
        Vector3 target = new Vector3(Random.Range(-_targetRangeX, _targetRangeX), 0f, 0f);
        bool isLarge = Random.value < _largeChance;

        GameObject meteor = PoolManager.Instance.Get(PoolType.Meteor, start, Quaternion.identity);
        meteor.GetComponent<Meteor>().Launch(target - start, _meteorSpeed, isLarge);
    }
}