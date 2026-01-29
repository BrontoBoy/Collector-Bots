using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(GoldsPool))]
public class GoldsSpawner : Spawner<Gold>
{ 
    [SerializeField] private float _spawnDelay = 2f;
    
    private Coroutine _spawnCoroutine;
    
    public event Action<Gold> GoldSpawned;
    
    public void StartSpawning()
    {
        StopSpawning();
        _spawnCoroutine = StartCoroutine(AutoSpawnRoutine());
    }
    
    public void StopSpawning()
    {
        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }
    }
    
    private IEnumerator AutoSpawnRoutine()
    {
        var wait = new WaitForSeconds(_spawnDelay);

        while (enabled)
        {
            SpawnRandom();
            yield return wait;
        }
    }
    
    protected override void OnObjectSpawned(Gold spawnedObject)
    {
        base.OnObjectSpawned(spawnedObject);
        GoldSpawned?.Invoke(spawnedObject);
    }
    
    public void ReturnGold(Gold gold)
    {
        if (GameObjectsPool != null && gold != null)
            GameObjectsPool.ReturnObject(gold);
    }
    
    private void OnDisable()
    {
        StopSpawning();
    }
}