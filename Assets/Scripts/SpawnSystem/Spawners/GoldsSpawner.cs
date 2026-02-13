using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class GoldsSpawner : MonoBehaviour
{
    [SerializeField] private GoldFactory _goldFactory;
    [SerializeField] private List<SpawnPoint> _spawnPoints;
    [SerializeField] private float _delay = 2f;

    private Coroutine _spawnCoroutine;

    private void Start()
    {
        StartSpawning();
    }
    
    private void OnDisable()
    {
        StopSpawning();
    }

    public void StartSpawning()
    {
        StopSpawning();
        _spawnCoroutine = StartCoroutine(AutoSpawnRoutine());
    }
    
    public void ReturnToPool(Gold gold)
    {
        if (_goldFactory != null)
            _goldFactory.Release(gold);
    }
    
    public void OnGoldCollected(Gold gold)
    {
        if (_goldFactory != null)
            _goldFactory.Release(gold);
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
        if (_goldFactory == null)
            return;
        
        SpawnPoint spawnPoint = GetRandomSpawnPoint();
        
        if (spawnPoint == null)
            return;
        
        _goldFactory.Create(spawnPoint.transform.position); 
    }
    
    private SpawnPoint GetRandomSpawnPoint()
    {
        if (_spawnPoints == null || _spawnPoints.Count == 0)
            return null;

        int randomIndex = Random.Range(0, _spawnPoints.Count);
        
        return _spawnPoints[randomIndex];
    }
}