using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(GoldsPool))]
public class GoldsSpawner : Spawner<Gold>
{ 
   [SerializeField] private float _delay = 2f;
    
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
    
    public void ReturnGold(Gold gold)
    {
        if (Pool != null && gold != null)
            Pool.ReturnObject(gold);
    }
    
    private IEnumerator AutoSpawnRoutine()
    {
        var wait = new WaitForSeconds(_delay);

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
    
    private void OnDisable()
    {
        StopSpawning();
    }
}