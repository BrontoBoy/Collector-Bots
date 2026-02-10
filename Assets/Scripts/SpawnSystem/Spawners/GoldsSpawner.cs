using System.Collections;
using UnityEngine;

public class GoldsSpawner : PoolSpawner<Gold>
{
    [SerializeField] private Gold _goldPrefab;
    [SerializeField] private float _delay = 2f;

    private Coroutine _spawnCoroutine;

    private void OnDisable()
    {
        StopSpawning();
    }

    public void StartSpawning()
    {
        StopSpawning();
        _spawnCoroutine = StartCoroutine(AutoSpawnRoutine());
    }

    private void StopSpawning()
    {
        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }
    }

    private IEnumerator AutoSpawnRoutine()
    {
        var wait = new WaitForSeconds(_delay);

        while (enabled)
        {
            SpawnGold();
            yield return wait;
        }
    }

    private void SpawnGold()
    {
        SpawnPoint spawnPoint = GetRandomSpawnPoint();
        
        if (spawnPoint == null || _goldPrefab == null)
            return;

        Gold gold = CreateInstance(_goldPrefab);
        gold.transform.position = spawnPoint.transform.position;
    }
}